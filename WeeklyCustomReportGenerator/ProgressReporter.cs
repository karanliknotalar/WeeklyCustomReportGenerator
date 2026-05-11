#nullable enable
using System;

namespace WeeklyCustomReportGenerator;

public static class ProgressReporter
{
    public static Action<int>? OnProgressChanged;
}