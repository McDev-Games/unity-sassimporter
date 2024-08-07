# SassImporter for Unity 
Original: https://github.com/arfinity-io/unity-sassimporter

## Not Ideal ##
After working a bit more with UI Toolkit, using this in production can cause confusion and furhter issues. Within the UI Builder one can edit a uss file and when that is done it will override the scss file and loss everything in it. Also when using the separate file mode then the changes of the uss file do not reflect in the scss file. I think that trading off the use of the WYSIWYG editor for SCSS is not beneficial.

## Info ##
This is a forked repository. I added an AssetPostprocessor script to simply reimport all *.scss files in the project (unless they begin with an underscore "_").
This helps the workflow when using @import as Unity will then catch the change and reimport the root *.scss files if a change to an imported file has been made.
Before that you had to manually reimport the root file.
## Installation

### Using the git repo

Add a git package in the Unity Package Manager with target:

https://github.com/McDev-Games/unity-sassimporter.git?path=/Package

## Usage

To use this package simply add it in the Unity Package Manager. Scss files are then automatically imported and available as UIToolkit stylesheets.

Make sure you have a [Sass](https://sass-lang.com) compiler locally installed. E.g., for Dart Sass, you can install it as a standalone version on

- Windows with Chocolatey

	```
	choco install sass
	```

- MacOS with Homebrew

	```
	brew install sass/sass/sass
	```

You can also install a Javascript implementation of Sass using Node

```
npm install -g sass
```

though this is said to be less performant.


## Functionality

The importer reads SCSS files such that they can be referenced like USS files. Make sure that you only use USS-supported properties, units, and queries in the SCSS code.

SCSS files starting with an underscore are ignored. These can be used with @import.

## Optional Extras

If you don't want to install sass locally on any machine that builds/imports this project there is an optional package 'io.arfinity.unity.packages.sassexecutables' that ships the sass executables for Linux/MacOS/Windows.
