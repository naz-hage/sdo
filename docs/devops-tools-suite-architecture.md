# DevOps Tools Suite Architecture

## Executable Details

**sdo.exe (Simple DevOps Operations Tool)**

* **Purpose**: Unified CLI utility for end-to-end build automation, repository management, work item tracking, and pipeline operations across Azure DevOps and GitHub.
* **Key Capabilities**:
* **Build & Utility Automation**: MSBuild integration, tool management (install/uninstall), environment setup, and GitHub release creation.
* **Work Item Management**: Full CRUD operations and commenting on Azure DevOps work items and GitHub issues.
* **Repository & PR Operations**: Multi-platform repo creation and deletion, git tagging/branching, and PR creation with branch-based auto-detection.
* **Pipeline Management**: CI/CD pipeline creation, execution, and monitoring.
* **Advanced Automation**: Auto-discovering YAML configuration system, Markdown parser for rich content generation, and dry-run mode.

### File Structure

```
sdo/
├── sdo.sln                    # Main solution file
├── prebuild.bat                  # Pre-build setup script
├── publish-local.ps1             # Local publishing script
├── mkdocs.yml                    # Documentation configuration
├── docs-requirements.txt         # Python dependencies for documentation
├── sdo-2e2-test.code-workspace  # VS Code workspace configuration
├── test-workflow.yml             # Test workflow configuration
│
├── CHANGELOG.md                  # Change log
├── README.md                     # Project documentation
├── targets.md                    # Build targets documentation
├── coverage.cobertura.xml        # Test coverage report
├── sdo.targets                # MSBuild targets
├── unit-tests.targets            # Unit test targets
│
├── Sdo/                          # sdo (C# executable) - Simple DevOps Operations Tool
│   ├── Sdo.csproj                # Consolidated production project
│   ├── ApiVersions/              # API version management
│   ├── GitHubRelease/            # GitHub integration
│   ├── Nbuild/                   # Build orchestration
│   ├── NbuildTasks/              # MSBuild tasks and utilities
│   ├── Nbackup/                  # Backup functionality
│   ├── Lf/                       # File-listing functionality
│   ├── Program.cs                # Main entry point and CLI setup
│   ├── Commands/                 # Command implementations
│   │   ├── WorkItemCommand.cs
│   │   ├── RepositoryCommand.cs
│   │   ├── PullRequestCommand.cs
│   │   └── PipelineCommand.cs
│   ├── Services/                 # Business logic
│   │   ├── WorkItemService.cs
│   │   ├── RepositoryService.cs
│   │   ├── PullRequestService.cs
│   │   └── PipelineService.cs
│   ├── Platforms/                # Platform implementations
│   │   ├── IPlatform.cs
│   │   ├── AzureDevOpsPlatform.cs
│   │   └── GitHubPlatform.cs
│   ├── Models/                   # Data models
│   ├── Credentials/              # Authentication handling
│   ├── Helpers/                  # Utility helpers
│   └── bin/Debug/sdo.exe         # Compiled executable
│
├── SdoTests/                     # Consolidated unit-test project
│   ├── SdoTests.csproj
│   ├── Nbuild/                   # Build and CLI tests
│   ├── NbuildTasks/              # MSBuild task tests
│   ├── GitHubRelease/            # Release service tests
│   ├── Nbackup/                  # Backup tests
│   └── Lf/                       # File-listing tests
│
├── docs/                         # Documentation (MkDocs)
│   ├── index.md
│   ├── nbuild.md                 # Nbuild documentation
│   ├── devops-tools-suite-architecture.md
│   ├── sdo.md
│   ├── sdo-net.md
│   └── other-docs/
│
├── dev-setup/                    # Development setup scripts
│   ├── sdo.yaml               # Application manifest
│   ├── apps.yaml                 # Tool definitions
│   └── setup scripts/
│
├── atools/                       # Automated tools and installers
│   ├── install-sdo.py         # SDO installation script
│   ├── requirements.txt           # Python dependencies for installers
│   ├── requirements-dev.txt       # Development dependencies
│   └── tests/                    # Tests for installation scripts
│       └── test_install_sdo.py
│
├── CoverageReport/               # Test coverage reports
│   ├── index.html
│   └── coverage data/
│
├── site/                         # Generated documentation site
│   └── (MkDocs output)
│
├── Release/                      # Release build outputs
│   └── published artifacts/
│
├── Debug/                        # Debug build outputs
│   └── build artifacts/
│
├── ArtifactsFolder/              # Build artifacts
│   └── intermediate outputs/
│
├── scripts/                      # Utility scripts
│   └── build and deployment scripts/
│
└── (other support directories)/
```

### Build System

The solution contains one production assembly and one test assembly. Both are built using:
- .NET 10.0 SDK
- MSBuild
- Custom build targets (sdo.targets)
- Consolidated build tasks in `Sdo/sdo.dll`

---

## sdo (Simple DevOps Operations Tool) - Architecture & Design

### Overview

sdo is a .NET/C# command-line tool for work item creation and repository management across Azure DevOps and GitHub platforms. It provides comprehensive DevOps capabilities with high performance and native .NET ecosystem integration.

### Architecture Principles

All SDO components follow consistent design principles:

#### 1. **Separation of Concerns**
- **CLI Layer**: User interaction and command handling
- **Business Logic**: Domain-specific orchestration and validation
- **Platform Layer**: Platform-specific implementations with abstract interfaces
- **Service Layer**: High-level business operations

#### 2. **Strategy Pattern**
- Abstract platform interfaces enable seamless switching between DevOps platforms
- Platform-specific implementations handle unique API requirements
- Consistent interfaces across all supported platforms

#### 3. **Extensibility**
- Plugin-style architecture for adding new platforms
- Modular service systems for different business domains
- Configurable authentication and API integration

### Advanced Automation Features

**Advanced Automation Features** introduces comprehensive enterprise capabilities to the SDO CLI tool, enabling teams to standardize operations and ensure reliable cross-platform testing.

#### 1. **YAML Configuration System**
- **ConfigurationManager**: New utility class with YamlDotNet integration
- **Auto-Discovery**: Searches current directory → .temp subfolder → parent directory's .temp folder
- **Nested Structure**: Supports complex nested YAML configurations flattened to dot-notation
- **Configuration Priority** (highest to lowest):
  1. CLI parameters (e.g., `sdo wi list --state "Done"`)
  2. Config file defaults (from sdo-config.yaml)
  3. Hard-coded defaults in code
- **Example**: `sdo wi list --config .\.temp\sdo-config.yaml` applies default filters for area, state, type, iteration, and top
- **Benefits**: Team standardization, reduced repetitive CLI arguments, config file sharing via version control

#### 2. **Markdown Parser**
- **MarkdownParser**: New utility class for parsing markdown content
- **Rich Content Support**: 
  - YAML frontmatter parsing
  - Title and description extraction
  - Metadata extraction (Key: Value format in headers)
  - Acceptance criteria parsing (checkbox-based lists)
  - Code block extraction and preservation
- **Features**:
  - GitHub-flavored markdown (GFM) syntax support
  - HTML sanitization for security
  - Comprehensive error reporting with line numbers
  - Verbose mode for detailed diagnostics
- **Integration**: Automatically applied when creating work items or PRs from markdown files
- **Benefits**: Professional-formatted work items, template reuse, standardized content structure

#### 4. **Service Enhancements**
- **AzureDevOpsClient**: 
  - Endpoint prioritization: /connectionData first (most reliable), falls back to Graph API
  - Enhanced error handling and logging
  - Improved user detection reliability
- **GitHubClient**: 
  - Enhanced error handling for API operations
  - Improved API timeout management (default 30s, configurable)
  - Better exception messaging for troubleshooting
- **WorkItemCommand**: 
  - Comprehensive error handling for list, show, create operations
  - Config file loading with error validation
  - Better user feedback and validation
- **PullRequestCommand**: 
  - Branch-based auto-detection: extracts work item ID from branch name pattern `<number>-<description>`
  - Auto-constructs file path as `.temp/<work-item-id>-pr-message.md` when not provided
  - Supports explicit overrides for file path and work item ID
  - Enhanced error messages with format examples and current branch context
  - Branch existence validation before creation
  - Improved error messages and diagnostic information
- **Benefits**: Simplified PR creation workflow, better error handling, improved reliability, enhanced developer experience

### Key Features

- **Multi-Platform Support**: Works seamlessly with Azure DevOps and GitHub
- **Work Item Management**: Create, list, show, update, and comment on work items
- **Repository Operations**: Create, list, delete, and manage repositories
- **Pull Request Management**: Create (with branch-based auto-detection), list, show, and update pull requests
- **Pipeline Management**: Create, run, monitor, and manage CI/CD pipelines
- **Dry-Run Mode**: Preview operations before execution
- **Automatic Platform Detection**: Detects platform from Git remote configuration
- **Branch-Based PR Creation**: Auto-detects work item ID and file path from branch name pattern
- **Configuration Management**: YAML-based defaults for standardized operations (Advanced Automation Features)
- **Rich Content**: Markdown templates for professional work items and PRs (Advanced Automation Features)

### Dependencies

#### SDO Build and Test Components
- **Runtime**: .NET 10.0 SDK
- **CLI Framework**: System.CommandLine 2.0.2
- **Build System**: YAML workflows with `sdo.yaml`

#### sdo (C# Simple DevOps Operations Tool)
- **Runtime**: .NET 10.0 (only supported implementation)
- **CLI Framework**: System.CommandLine 2.0.2
- **Configuration**: YamlDotNet 15.1.1 for YAML parsing (Advanced Automation Features)
- **API Clients**: Octokit.NET (GitHub API), Microsoft.TeamFoundationServer.Client (Azure DevOps API)
- **HTTP**: System.Net.Http for REST calls
- **Development & Testing**: xunit, Moq, Microsoft.NET.Test.Sdk
- **Installation Scripts**: Python (for cross-platform installer compatibility only)

## Integration Points

### SDO Workflows
1. **Build → Work Item Creation**: SDO builds can trigger work item creation
2. **Repository Management**: SDO can create repositories and build them
3. **Pipeline Integration**: SDO pipeline operations integrate with build automation
4. **Work Item Tracking**: SDO provides work item management alongside build tracking

### Shared Concepts
- **Dual Platform Support**: Both tools work seamlessly with Azure DevOps and GitHub
- **Common Authentication Patterns**: Shared credential management across SDO commands
- **Consistent CLI Design**: System.CommandLine framework across SDO commands
- **Cross-Platform Compatibility**: All executables run on Windows, Linux, and macOS
- **Performance First**: C# implementation ensures fast, reliable DevOps operations
