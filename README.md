# Performance Optimizations in C#

Slides and benchmarks for my talk. The slides are in `Powerpoint.pptx`, the benchmarks in `Benchmarks/` and the LLM prompts in `prompts/`.

## Run the benchmarks

You need the .NET 10 SDK and the .NET 6 runtime (for the .NET 6 vs .NET 10 comparison).

```bash
cd Benchmarks
dotnet run -c Release -f net10.0
```

Pick a benchmark from the list, or pass a filter: `dotnet run -c Release -f net10.0 -- --filter *UnitPrice*`

The numbers on the slides are from an AMD Ryzen 9 5950X on Windows 11 with .NET 10.0.12 and .NET 6.0.36.
