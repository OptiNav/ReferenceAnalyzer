﻿using FluentAssertions;
using Microsoft.Reactive.Testing;
using Moq;
using ReactiveUI.Testing;
using ReferenceAnalyzer.Core;
using ReferenceAnalyzer.Core.Util;
using ReferenceAnalyzer.WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Xunit;

namespace ReferenceAnalyzer.WPF.Tests
{
    public class AppViewModelTests
    {
        private const string _Path = "samplePath";
        private Mock<ISettings> _SettingsMock;
        private Mock<IReferenceAnalyzer> _providerMock;
        private AppViewModel _Sut;
        private List<ReferencesReport> _Projects;
        private List<ActualReference> _ReferencedProjects;
        private TestScheduler _Scheduler;

        public AppViewModelTests() => new TestScheduler().With(scheduler =>
        {
            _Scheduler = scheduler;
            _providerMock = new Mock<IReferenceAnalyzer>();
            _ReferencedProjects = new List<ActualReference>
            {
                new ActualReference("project1", Enumerable.Empty<ReferenceOccurrence>())
            };

            _Projects = new List<ReferencesReport>
            {
                new ReferencesReport("proj1", new [] {"ref1","ref2" }, _ReferencedProjects),
                new ReferencesReport("proj2", new [] {"ref2", "ref3"}, null)
            };
            _providerMock.Setup(m => m.AnalyzeAll(_Path))
                .Returns(_Projects.ToAsync());

            _SettingsMock = new Mock<ISettings>();
            _SettingsMock.Setup(x => x.SolutionPath).Returns(_Path);
            _SettingsMock.SetupSet(x => x.SolutionPath = It.IsAny<string>()).Verifiable();

            _Sut = new AppViewModel(_SettingsMock.Object, _providerMock.Object);
        });

        [Fact]
        public void Instantiates()
        {
            Action a = () => new AppViewModel(_SettingsMock.Object, _providerMock.Object);

            a.Should().NotThrow();
        }

        [Fact]
        public void NoPathButtonDisabled()
        {
            var canExecute = true;
            _Sut.Path = "";
            _Sut.Load.CanExecute.Subscribe(x => canExecute = x);
            canExecute.Should().Be(false);
        }

        [Fact]
        public void CorrectPathSetEnabled()
        {
            var canExecute = false;
            _Sut.Path = "C:/Path";
            _Sut.Load.CanExecute.Subscribe(x => canExecute = x);
            canExecute.Should().Be(true);
        }

        [Fact]
        public void LoadedServiceInvoked()
        {
            var path = _Path;
            _Sut.Path = path;
            _Sut.Load.Execute().Subscribe();

            _providerMock.Verify(x => x.AnalyzeAll(path));
        }

        [Fact]
        public void LoadedServiceListUpdated()
        {
            _Sut.Path = _Path;
            _Sut.Load.Execute().Subscribe();

            _Scheduler.AdvanceBy(3);

            _Sut.Projects.Should().NotBeNullOrEmpty();
            _Sut.Projects.Should().BeEquivalentTo(_Projects);
        }

        [Fact]
        public void LoadedProjectSelectedShowsReferenceList()
        {
            _Sut.Load.Execute().Subscribe();

            _Scheduler.AdvanceBy(3);

            _Sut.SelectedProject = _Sut.Projects.First();

            _Sut.SelectedProject.DefinedReferences.Should().NotBeNull();
        }

        [Fact]
        public void DefaultSolutionPathTakenFromSettings()
        {
            var expected = _Path;

            _Sut.Path.Should().Be(expected);
        }

        [Fact]
        public void ChangingPathSavedInSettings()
        {
            var newPath = "newPath";
            _Sut.Path = newPath;

            _SettingsMock.VerifySet(x => x.SolutionPath = newPath);
        }

    }
}
