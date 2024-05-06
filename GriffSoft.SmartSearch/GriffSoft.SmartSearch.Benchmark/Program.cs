using BenchmarkDotNet.Running;

using GriffSoft.SmartSearch.Benchmark;

// Console output can nicely be visualised with https://chartbenchmark.net/
_ = BenchmarkRunner.Run<Benchmark>();
