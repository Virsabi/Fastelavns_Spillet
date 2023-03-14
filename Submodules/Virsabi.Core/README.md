# VirsabiRepoDevelopment
[![Version](https://img.shields.io/badge/dynamic/json?color=blue&label=Release&query=version&url=https%3A%2F%2Fraw.githubusercontent.com%2FVirsabi%2FVirsabiPublicFiles%2Fmaster%2FVirsabi.Core%2Fpackage.json)](https://github.com/Virsabi/Virsabi.Core/releases)
[![Version](https://img.shields.io/badge/dynamic/json?color=brightgreen&label=Unity%20Version&query=unity&suffix=%20or%20later&url=https%3A%2F%2Fraw.githubusercontent.com%2FVirsabi%2FVirsabiPublicFiles%2Fmaster%2FVirsabiRepoDevelopment%2Fpackage.json)](https://github.com/Virsabi/Virsabi.Core/releases)
[![Submodule](https://img.shields.io/badge/Included%20Submodule-MyBox-blue)](https://github.com/Deadcows/MyBox)
[![Actions Status](https://github.com/Virsabi/VirsabiRepoDevelopment/workflows/CI/badge.svg)](https://github.com/Virsabi/Virsabi.Core/actions)
#### Internal Repo - If you can read this you are on a non-upm branch - This branch is for development.

The upm releases are on a subtree being synched with a respective develop branch:
- pushes to 'releaseDev' synchronises the folder 'Packages/com.virsabi' to the 'upmDev' subtree. 
- pushes to 'master' synchronises the folder 'Packages/com.virsabi' to the 'upm' subtree. 
	- This also starts an automatic release action which creates a release based on previously correctly formatted commits. 

Therefore;
- Develop features on a new branch.
- When you want to test your features through a different project push it to 'releaseDev'
	- This will update the upmDev branch (can take up to 2 minutes), which can be installed in unity using https://github.com/Virsabi/VirsabiRepoDevelopment.git#upmDev
- When you have tested the features on a client project (package installed through package manager using git) then you can push to the 'master' branch to update the 'upm' branch.
	- After actions have completed you can checkout 'releaseDev' and pull from master, then push to 'releaseDev' to update the changelog and version of the 'upmDev' branch.
- In order for our automatic release to create changelogs correctly write commits as explained in the next section. Read more about the commit message guidelines [here](https://github.com/angular/angular.js/blob/master/DEVELOPERS.md#-git-commit-guidelines).

IMPORTANT: NEVER push directly to any upm branch!

### Commit Message Format:
If the prefix is feat, fix or perf, it will trigger a release and appear in the changelog. This is **CASE SENSITIVE**.

Examples:

`fix: Fix a bug` will create a patch release. For instance 1.5.**4** will become 1.5.**5**
> Patch version Z (x.y.Z | x > 0) MUST be incremented if only backwards compatible bug fixes are introduced. A bug fix is defined as an internal change that fixes incorrect behavior.

`feat: Add a nice feature` will create a minor release. For instance 1.**5**.4 will become 1.**6**.0
> Minor version Y (x.Y.z | x > 0) MUST be incremented if new, backwards compatible functionality is introduced to the public API. It MUST be incremented if any public API functionality is marked as deprecated. It MAY be incremented if substantial new functionality or improvements are introduced within the private code. It MAY include patch level changes. Patch version MUST be reset to 0 when minor version is incremented.

`docs: Updated the readme` will not create any release.

We havn't had the need to work with major versions, but just for the sake of context here is the official desciption:
> Major version X (X.y.z | X > 0) MUST be incremented if any backwards incompatible changes are introduced to the public API. It MAY also include minor and patch level changes. Patch and minor version MUST be reset to 0 when major version is incremented.

Other prefixes are up to your discretion. Suggested prefixes are docs, style, refactor, and test for non-changelog related tasks that doesn't trigger a release.


## Developing Features on our package:
Scripts should always be associated with an assembly reference and have their own Virsabi.xxx name space relevant for the feature.
Try to keep features independent of other assmeblies and libraries.

(Note; Scripts that write code directly in other scritps or files (like the SceneOpenerWindows system) needs to utilize the samples folder, since the package folder is read-only when downloaded through git pacakge manager).
Also, if you are writing features that implement user specific settings (like keyboard shortcuts or toggleable editor settings) you can use the VirsabiSettings.cs script to save persitently correctly. 

When creating features that implements samples put them in the sample folder and add the sample to the package.json file.
Also, example scripts included in the sample folder need assembly references or they wont work!

When moving files (whether its scripts, assets or something else) from assets to the pacakge make sure all references are set correctly in your sample scenes - eg. there is nothing in the scene pointing to assets in the /assets/. folder. Otherwise they won't be pacakged and the sample scene will have missing references on an upm unity client.

### Psuedo Procedure for adding features from your project to the repo package
- Make sure your project has added the Virsabi pacakge from disk and not from git (otherwise you cannot edit it)
	- You can check this in the updater window of the virsabi tool or in the manifest.json file of your project.
	- Also, it can create bugs if multiple untiy projects have added the project from disk since they can overwrite eachother's meta files - Safest way is to only have one project for developing on our package.
- Make sure the package on disk is up to date: Branch off master or releaseDev. 
- Move over your assets to a new folder describing the feature under either editor or runtime.
		-if your have some scripts for runtime and some for editor, your whole feature should go under runtime - don't split the feature into editor and runtime.
	- If you have any scripts make sure to add an .asmdef setup correctly.
	- Move any example scenes, scripts or assets to the sample folder and add them to the package.json file.
		- Again make sure to add an .asmdef file if using sample scripts.
- Test functionality and build capability in your project.
- Commit and push to releaseDev; wait a few minutes (until subtree action has completed).
- Download/Update the package in another unity project using the #upmDev branch.
	- This allows you to test in a clean/different environment than your origin project.
- Follow the test guide below to make sure everything works.
- Once all is good, push to releaseDev to master in order to create a new release on the upm branch.

## Testing the Package:
- Make sure scripts are compiled correctly and using Scripting Define Symbols correctly
	- test in client project by using the upmDev branch method.
	- Also test build capacity - some compile errors only show up when attempting to build.
	- Make sure you can compile/build the project with and without your features enabled using Scripting Define Sympols.
- Make sure sample scenes work correctly - test both in your development environment and on a client using umpDev before pushing to master.


_________
This repo follows the upm development workflow explained [here](https://medium.com/openupm/how-to-maintain-upm-package-part-1-7b4daf88d4c4)

Badges created following [this guide](https://medium.com/@vemarav/dynamic-badges-using-shields-io-5948dcb2a99d) - badages for private repos are almost impossible. Action run on master branch using [copycat action](https://github.com/andstor/copycat-action) makes our package.json file public on [another repo](https://github.com/Virsabi/VirsabiPublicFiles) for badge reading.
