# MarkdownDirConvert
Convert directory of .md to .html

Usage:
- Run with one parameter (source directory). Converted directory is placed next to source one with same name and "_converted" appended.
- Or run with two parameters: source directory and destination directory.

How it works:
- Directory structure copied, i.e. subdirectories, including images and other files.
- Markdown files are parsed and links to .md files replaced with .html
- Html files generated
  - Also heading structure created
  
Uses great [Markdig library](https://github.com/lunet-io/markdig).

Inspired by [MarkdownEditor](https://github.com/madskristensen/MarkdownEditor) Visual Studio extension.
