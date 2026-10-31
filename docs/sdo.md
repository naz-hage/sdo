## Tool version automation

Tool versions in this table are automatically updated from the single `apps.yaml` file (located at `dev-setup/apps.yaml`). To update the table, run:

```
sdo tool manifest update --latest --file-path .\docs\sdo.md
```

This will update the manifest from the latest stable releases, then extract all tool/version pairs from the `NbuildAppList` entries and update the documentation table. The `--file-path` option accepts any Markdown file containing the version table. The `dev-setup/apps.yaml` file serves as the **single source of truth** for all developer tools managed by SDO. No PowerShell script is needed or maintained for this process.
The [Windows dev environment](https://learn.microsoft.com/en-us/windows/dev-environment/) has good information on how to setup a Windows dev environment.

- The table below lists the latest development tools used by SDO.

| Tool                                                                                                       | Version     | Last Checked on |
| :--------------------------------------------------------------------------------------------------------- | :---------- | :-------------- |
| [Argo CD](https://github.com/argoproj/argo-cd/releases/) | 3.5.6      | 30-Oct-26      |
| [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?pivots=msi)             | 2.78.0     | 30-Oct-26      |
| [Burp Suite](https://portswigger.net/burp/communitydownload)                                              | 2021.11.2   | 01-Oct-23       |
| [Dotnet Runtime](https://dotnet.microsoft.com/en-us/download/dotnet) | 9.0.2      | 30-Oct-26      |
| [Dotnet8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)                                         | 8.0.408     | 02-May-25       |
| [Dotnet9 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet)                                     | 9.0.203     | 02-May-25       |
| [Draw.io](https://app.diagrams.net/)                                                                      | N/A         | 01-Oct-23       |
| [Git for Windows](https://git-scm.com/downloads) | 2.56.0     | 30-Oct-26      |
| [Install Docker Desktop on Windows](https://docs.docker.com/docker-for-windows/install/)                  | 4.38.0.0   | 07-Sep-25      |
| [kubernetes](https://github.com/kubernetes/kubernetes/releases)                                           | 1.37.2     | 30-Oct-26      |
| [minikube](https://github.com/kubernetes/minikube/releases/) | 1.39.0     | 30-Oct-26      |
| [MongoDB Community Server](https://www.mongodb.com/try/download/community)                                | 9.0.4      | 30-Oct-26      |
| [Node.js](https://nodejs.org/en/download/) | 22.21.0    | 30-Oct-26      |
| [SDO](https://github.com/naz-hage/sdo/releases) | 1.47.0     | 24-May-26     |
| [GitHub CLI](https://github.com/cli/cli/releases) | 2.103.3    | 30-Oct-26      |
| [pnpm](https://pnpm.io/) | 12.16.1    | 30-Oct-26      |
| [Postman Get Started for Free](https://www.postman.com/downloads/)                                        | v11.36.0    | 10-Mar-25       |
| [PowerShell](https://github.com/PowerShell/PowerShell/releases) | 7.6.9      | 30-Oct-26      |
| [Python](https://www.python.org/downloads/) | 3.14.0     | 30-Oct-26      |
| [SysInternals](https://learn.microsoft.com/en-us/sysinternals/) | 2.90.0.0   | 30-Oct-26      |
| [Terraform](https://releases.hashicorp.com/terraform) | 1.13.4     | 30-Oct-26      |
| [Terraform Lint](https://github.com/terraform-linters/tflint/releases) | 0.65.0     | 30-Oct-26      |
| [Visual Studio Code](https://code.visualstudio.com/download) | 1.144.0    | 30-Oct-26      |
| [Windows Terminal](https://www.microsoft.com/en-us/p/windows-terminal/9n0dx20hk701)                       | 1.21.10351.0| 10-Mar-25       |
