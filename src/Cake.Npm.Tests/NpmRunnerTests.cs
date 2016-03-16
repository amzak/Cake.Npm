﻿
using System;

using Shouldly;

using Xunit;

namespace Cake.Npm.Tests {
	public class NpmRunnerTests {
		private readonly NpmRunnerFixture fixture;

		public NpmRunnerTests() {
			this.fixture = new NpmRunnerFixture();
		}

		[Fact]
		public void No_Install_Settings_Should_Use_Correct_Argument_Provided_In_NpmInstallSettings() {
			this.fixture.InstallSettings = null;

			var result = this.fixture.Run();

			result.Args.ShouldBe("install");
		}

		[Fact]
		public void Globally_Settings_Specified_Should_Use_Correct_Arguments() {
			this.fixture.InstallSettings = s => s.Globally();
			var result = this.fixture.Run();

			result.Args.ShouldBe("install --global");
		}

		[Fact]
		public void ForProduction_Settings_Specified_Should_Use_Correct_Arguments() {
			this.fixture.InstallSettings = s => s.Package("abc").ForProduction();

			Action run = () => this.fixture.Run();

			run.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Packages_And_ForProduction_Settings_Specified_Should_Use_Throw_ArgumentException() {
			this.fixture.InstallSettings = s => s.ForProduction();

			var result = this.fixture.Run();

			result.Args.ShouldBe("install --production");
		}

		[Theory]
		[InlineData("any package")]
		[InlineData("https://path.co/package/v0.1")]
		public void Package_Settings_Specified_Should_Use_Correct_Arguments(string package) {
			this.fixture.InstallSettings = s => s.Package(package);

			var result = this.fixture.Run();

			result.Args.ShouldBe("install " + package);
		}

		[Fact]
		public void Package_With_Tag_Settings_Specified_Should_Use_Correct_Arguments() {
			this.fixture.InstallSettings = s => s.Package("any package", ">1.0 && <1.5");

			var result = this.fixture.Run();

			result.Args.ShouldBe("install any package@\">1.0 && <1.5\"");
		}

		[Fact]
		public void Package_With_Tag_And_Scope_Settings_Specified_Should_Use_Correct_Arguments() {
			this.fixture.InstallSettings = s => s.Package("any package", ">1.0 && <1.5", "@scope");

			var result = this.fixture.Run();

			result.Args.ShouldBe("install @scope/any package@\">1.0 && <1.5\"");
		}

		[Fact]
		public void Package_With_Tag_And_Invalid_Scope_Settings_Specified_Should_Throw_ArgumentException() {
			this.fixture.InstallSettings = s => s.Package("any package", ">1.0 && <1.5", "scope");

			Action run = () => this.fixture.Run();

			run.ShouldThrow<ArgumentException>();
		}

		[Theory]
		[InlineData("any package")]
		[InlineData("https://path.co/package/v0.1")]
		public void Package_And_Globally_Settings_Specified_Should_Use_Correct_Arguments(string package) {
			this.fixture.InstallSettings = s => s.Package(package).Globally();

			var result = this.fixture.Run();

			result.Args.ShouldBe("install " + package + " --global");
		}
	}
}