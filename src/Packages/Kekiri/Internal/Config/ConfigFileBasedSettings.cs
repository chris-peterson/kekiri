using System;

namespace Kekiri.Internal.Config;

// removed support for clients specifying this, but since it was developed as a config section, keep it around JIC.
internal class ConfigFileBasedSettings
{
    public string Given => "Given";

    public string When => "When";

    public string Then => "Then";

    public string Line => "\r\n";

    public string Indent => "  ";

    public string And => "And";

    public string But => "But";
}
