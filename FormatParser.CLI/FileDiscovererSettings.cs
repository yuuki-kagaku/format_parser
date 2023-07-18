namespace FormatParser.CLI;

public record FileDiscovererSettings(bool FallOnUnauthorizedException = false, bool FailOnIOException = true);