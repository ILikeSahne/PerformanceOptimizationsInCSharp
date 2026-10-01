using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance.HideColumns(
    Column.Error,
    Column.StdDev,
    Column.Median,
    Column.RatioSD,
    Column.Gen0,
    Column.Gen1,
    Column.Gen2);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
