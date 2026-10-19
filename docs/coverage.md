# Coverage

Kekiri is tested with [TUnit](https://tunit.dev) rather than with itself, so a defect in the runner
can't hide the failures that would reveal it. The numbers below come from that suite alone, measured
on every push to `main`.

Every package is held to **90% of lines and 90% of branches**. A branch is one outcome of a decision
(either side of an `if`, each `case`, each short-circuit of `&&`, `||` and `??`), so branch coverage
is what shows a condition whose other side never ran. CI fails a change that takes any package below
either number.

[summary](coverage/summary.md ':include')

The [line-by-line report](https://chris-peterson.github.io/kekiri/coverage/report/index.html) marks
the lines of every file that no test ran, and with a `~` the lines that ran with a branch left over.
It counts both as missed, so its per-file numbers run lower than the line numbers above.

Coverage is collected by [coverlet](https://github.com/coverlet-coverage/coverlet) and the
line-by-line report rendered by [pycobertura](https://github.com/aconrad/pycobertura), both MIT
licensed. To measure it locally, from a clone:

```bash
python3 scripts/coverage.py            # the table above, and the gate
python3 scripts/coverage.py --html     # plus the line-by-line report (pip install pycobertura==4.1.0)
```

It writes to `artifacts/coverage/` and exits non-zero when a package is below the gate.
