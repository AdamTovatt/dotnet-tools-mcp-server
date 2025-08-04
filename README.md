# DotNet Tools MCP Server

An MCP (Model Context Protocol) server for building and testing .NET projects using `dotnet build` and `dotnet test` from Cursor.

This server provides a workaround for Cursor's current terminal issues when trying to build and test .NET projects directly.

## Installation

You can either:
- Download a prebuilt release. It's just a single executeable file with everything that it needs to run inside of it.
- Build from source using `dotnet publish` (AOT compiled into a single file, must be configured for the correct platform)

## Configuration

Add this to your Cursor settings to use the MCP server:

```json
{
  "mcpServers": {
    "dotnet-tools": {
      "name": "DotNet Tools Server",
      "stdio": true,
      "command": "C:\\path\\to\\your\\DotNetToolsMcpServer.exe"
    }
  }
}
```

Replace the command path with the actual location of the executable on your system.

## Usage

### MCP Server Mode

When run without command line arguments, the server operates as an MCP server for Cursor integration.

### CLI Mode

When run with command line arguments, the server operates as a CLI tool for managing library documentation configuration.

#### CLI Commands

- `add-library --name="LibraryName" --url="https://..."` - Add a library documentation URL
- `remove-library --name="LibraryName"` - Remove a library from configuration
- `list-libraries` - List all configured libraries
- `config-path` - Display the path to the configuration file
- `open-config` - Open the configuration file in the default editor

#### Help

- `-h`, `-help`, `--help` - Show help information

#### Examples

```bash
# Show help
DotNetToolsMcpServer -h
DotNetToolsMcpServer --help

# Add a library
DotNetToolsMcpServer add-library --name="EasyReasy" --url="https://raw.githubusercontent.com/AdamTovatt/easy-reasy/refs/heads/master/EasyReasy/README.md"

# List all libraries
DotNetToolsMcpServer list-libraries

# Show config file location
DotNetToolsMcpServer config-path

# Open config file for editing
DotNetToolsMcpServer open-config
```

#### Configuration File

The configuration is stored in a local markdown file at:
- **Windows**: `%APPDATA%\DotNetToolsMcpServer\libraries.md`
- **macOS/Linux**: `~/.config/DotNetToolsMcpServer/libraries.md`

The file uses markdown format:
```markdown
- [LibraryName](https://url-to-documentation.md)
```

## Tools

This MCP server provides the following tools:
- `mcp_dotnet-tools_build` - Builds a .NET project using `dotnet build`
- `mcp_dotnet-tools_build_solution` - Builds a .NET solution using `dotnet build`
- `mcp_dotnet-tools_run_tests` - Runs all tests in a .NET test project
- `mcp_dotnet-tools_run_specific_tests` - Runs specific tests using custom filters
- `mcp_dotnet-tools_list_available_documentation_files` - Lists all available documentation files
- `mcp_dotnet-tools_get_documentation_for_library` - Gets documentation for a specific library by NuGet package name

## Library Documentation

The server supports user-configurable library documentation. Instead of using a hardcoded URL, users can add their own library documentation files through the CLI commands. The documentation tool will read from the local configuration file to provide documentation for configured libraries. 