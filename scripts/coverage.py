#!/usr/bin/env python3
"""Run the test suite with coverage and enforce the coverage gate.

Usage: python3 scripts/coverage.py [--out DIR] [--min-lines PCT] [--min-branches PCT] [--html]

Coverage is collected by coverlet (coverlet.MTP, referenced by the test project). Writes, under
--out (default artifacts/coverage):
  coverage.cobertura.xml     the raw coverage data
  summary.md                 the per-package table the docs coverage page includes
  lines.json, branches.json  shields.io endpoint badges
  report/index.html          with --html: pycobertura's line-by-line report
                             (pip install pycobertura==4.1.0)

Exit codes:
  0  every package meets both thresholds
  1  a package is below a threshold, or missing from the report
  2  a build, test, or report step failed (its own output says why)
"""

import argparse
import json
import pathlib
import shutil
import subprocess
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict
from datetime import datetime, timezone

PACKAGES = ["Kekiri", "Kekiri.Mtp", "Kekiri.IoC.Autofac", "Kekiri.IoC.ServiceProvider"]
REPO = pathlib.Path(__file__).resolve().parent.parent


def run(*command):
    if subprocess.run(command, cwd=REPO).returncode != 0:
        print(f"coverage: '{' '.join(command)}' failed", file=sys.stderr)
        sys.exit(2)


def measure(cobertura):
    """Per-package (covered lines, lines, covered branches, branches).

    A line can appear under its class and again under each method, so each (file, line) is
    counted once.
    """
    totals = {}
    for package in ET.parse(cobertura).getroot().iter("package"):
        lines = defaultdict(lambda: (0, 0, 0))
        for cls in package.iter("class"):
            for line in cls.iter("line"):
                key = (cls.get("filename"), line.get("number"))
                hits = int(line.get("hits"))
                covered, total = 0, 0
                condition = line.get("condition-coverage")
                if condition:
                    covered, total = (int(n) for n in condition.split("(")[1].rstrip(")").split("/"))
                old = lines[key]
                lines[key] = (max(old[0], hits), max(old[1], covered), max(old[2], total))
        totals[package.get("name")] = (
            sum(1 for hits, _, _ in lines.values() if hits > 0),
            len(lines),
            sum(covered for _, covered, _ in lines.values()),
            sum(total for _, _, total in lines.values()),
        )
    return totals


def percent(covered, total):
    return round(100 * covered / total, 1) if total else 100.0


def badge(label, value, minimum):
    color = "brightgreen" if value >= minimum else "red"
    return {"schemaVersion": 1, "label": label, "message": f"{value:g}%", "color": color}


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--out", default="artifacts/coverage")
    parser.add_argument("--min-lines", type=float, default=90)
    parser.add_argument("--min-branches", type=float, default=90)
    parser.add_argument("--html", action="store_true", help="also write pycobertura's HTML report")
    args = parser.parse_args()

    if args.html and shutil.which("pycobertura") is None:
        print("coverage: --html needs pycobertura: pip install pycobertura==4.1.0", file=sys.stderr)
        sys.exit(2)

    out = (REPO / args.out).resolve()
    results = out / "results"
    shutil.rmtree(results, ignore_errors=True)

    run("dotnet", "test", "-c", "Release",
        "--coverlet", "--coverlet-output-format", "cobertura",
        *[a for p in PACKAGES for a in ("--coverlet-include", f"[{p}]*")],
        "--results-directory", str(results))

    reports = sorted(results.glob("coverage.cobertura*.xml"))
    if len(reports) != 1:
        print(f"coverage: expected one cobertura file in {results}, found {len(reports)}", file=sys.stderr)
        sys.exit(2)
    cobertura = out / "coverage.cobertura.xml"
    shutil.copyfile(reports[0], cobertura)

    if args.html:
        report = out / "report"
        report.mkdir(parents=True, exist_ok=True)
        run("pycobertura", "show", "--format", "html", "--output", str(report / "index.html"), str(cobertura))

    measured = measure(cobertura)
    all_lines = [sum(measured[p][i] for p in PACKAGES if p in measured) for i in range(4)]
    total_lines = percent(all_lines[0], all_lines[1])
    total_branches = percent(all_lines[2], all_lines[3])

    (out / "lines.json").write_text(json.dumps(badge("line coverage", total_lines, args.min_lines)))
    (out / "branches.json").write_text(json.dumps(badge("branch coverage", total_branches, args.min_branches)))

    failures = []
    rows = []
    print(f"\n{'package':28} {'lines':>7} {'branches':>9}")
    for package in PACKAGES:
        if package not in measured:
            failures.append(f"{package} is missing from the report (no test loaded it)")
            print(f"{package:28} {'-':>7} {'-':>9}")
            rows.append(f"| `{package}` | missing | missing | below |")
            continue

        covered_lines, lines, covered_branches, branches_total = measured[package]
        lines_pct = percent(covered_lines, lines)
        branches_pct = percent(covered_branches, branches_total)
        print(f"{package:28} {lines_pct:6g}% {branches_pct:8g}%")
        below = lines_pct < args.min_lines or branches_pct < args.min_branches
        rows.append(f"| `{package}` | {lines_pct:g}% | {branches_pct:g}% | {'below' if below else 'met'} |")
        if lines_pct < args.min_lines:
            failures.append(f"{package} lines {lines_pct:g}% < {args.min_lines:g}%")
        if branches_pct < args.min_branches:
            failures.append(f"{package} branches {branches_pct:g}% < {args.min_branches:g}%")

    print(f"{'total':28} {total_lines:6g}% {total_branches:8g}%")

    (out / "summary.md").write_text("\n".join([
        f"| Package | Lines | Branches | Gate ({args.min_lines:g}% / {args.min_branches:g}%) |",
        "|:--|--:|--:|:--|",
        *rows,
        f"| **Total** | **{total_lines:g}%** | **{total_branches:g}%** | |",
        "",
        f"Measured {datetime.now(timezone.utc).strftime('%Y-%m-%d %H:%M UTC')}.",
        "",
    ]))

    if failures:
        sys.stdout.flush()
        print("\ncoverage gate failed:\n  " + "\n  ".join(failures), file=sys.stderr)
        sys.exit(1)

    print(f"coverage gate passed: every package at {args.min_lines:g}% lines, {args.min_branches:g}% branches or better")


if __name__ == "__main__":
    main()
