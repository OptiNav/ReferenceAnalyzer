using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReferenceAnalyzer.Core
{
    public interface IReferenceAnalyzer
    {
        IDictionary<string, string> BuildProperties { get; set; }
        bool ThrowOnCompilationFailures { get; set; }
        IProgress<double> ProgressReporter { get; set; }
        IAsyncEnumerable<ReferencesReport> AnalyzeAll(string solutionPath);
        Task<ReferencesReport> Analyze(string target);
        Task<IEnumerable<string>> Load(string solution);
        IAsyncEnumerable<ReferencesReport> AnalyzeAll();
        IAsyncEnumerable<ReferencesReport> Analyze(IEnumerable<string> projects);
    }
}
