# initproject

## Description
`initproject` is a command-line tool designed to simplify the process of renaming folders and files in a specified directory. This tool replaces a specified template name with a new name across all files and directories in the target directory, while also ensuring that Git is set up properly.

## Features
- **Directory Validation**: Ensures the target directory exists and has Git initialized.
- **Template Replacement**: Replaces occurrences of a specified template name with a new name in both folder and file names.
- **File Content Update**: Modifies the content of files to replace the template name with the new name.
- **Customizable Options**: Supports customizable command-line options for directory, template name, and file extensions.

## Prerequisites
- [.NET Framework](https://dotnet.microsoft.com/download/dotnet-framework) (version 4.8 or later)
- Git must be installed and accessible from the command line.

## Installation
1. Clone the repository:
   ```
   git clone https://github.com/watertrans/initproject.git
   ```

2. Navigate to the project directory:
   ```
   cd initproject
   ```

3. Build the project using your preferred method (e.g., Visual Studio or .NET CLI).

## Usage
To run the program, use the following command structure:
   ```
   initproject.exe --dir <directory> --name <new_name> [--template <template_name>] [--ext <extensions>]
   ```

## Options

- `--dir` or `-d`: Specify the target directory. (Required)
- `--name` or `-n`: Specify the name to replace the template name. (Required)
- `--template` or `-t`: Specify the template name to replace. (Optional, defaults to `__template__`)
- `--ext` or `-e`: Specify the file extensions to process (Optional, defaults to `.cs`, `.csproj`, `.vb`, `.vbproj`, `.config`, `.json`, `.aspx`, `.css`, `.js`, `.html`, `.sln`)

## Example
```
initproject.exe --dir "C:\Projects\MyProject" --name "MyNewProject"
```

## License
This project is licensed under the MIT License.
