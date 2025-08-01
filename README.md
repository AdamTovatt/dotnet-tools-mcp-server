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

## Tools

This MCP server provides the following tools:
- `mcp_dotnet-tools_build` - Builds a .NET project using `dotnet build`
- `mcp_dotnet-tools_build_solution` - Builds a .NET solution using `dotnet build`
- `mcp_dotnet-tools_run_tests` - Runs all tests in a .NET test project
- `mcp_dotnet-tools_run_specific_tests` - Runs specific tests using custom filters

## Usage

Once configured, Cursor can invoke these methods to build and test .NET projects directly without relying on the terminal. 