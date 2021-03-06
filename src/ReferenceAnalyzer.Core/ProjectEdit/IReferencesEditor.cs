using System.Collections.Generic;

namespace ReferenceAnalyzer.Core.ProjectEdit
{
    public interface IReferencesEditor
    {
        IEnumerable<string> GetReferencedProjects(string projectPath);
        IEnumerable<string> GetReferencedPackages(string projectPath);
        void RemoveReferencedProjects(string projectPath, IEnumerable<string> projects);
        string? GetOutputAssemblyName(string projectPath);
        void InvalidateCache();
    }
}
