# Contribution Requirements
Welcome to the contribution requirements! This document will outline the steps needed to submit and ultimately merge your changes into the live codebase.

## Getting Started
To start making contributions follow the steps below:

1. Discuss how you would like to contribute through an issue.
2. Fork the repository.
3. Create a branch based on the `master` branch or a feature branch.

Now that you're ready to contribute, below are some things to keep in mind to make sure the pull request gets approved quickly.

### Master vs. Feature Branches
This repository features multiple types of branches that have different requirements when making contributions to them. 

First is the `master` branch, which requires you to follow all the steps below to get your pull request approved. The `master` branch aligns with public releases, so it should have the highest quality code possible. 

The rest of the branches are considered *feature* branches. Contributions to feature branches have a less rigorous review process because they don't align with public releases. Once a feature is complete, it will have to follow all guidelines when merged with `master`.

### Testing
When working on bug fixes or adding new features, ensure you build tests and keep old tests passing to keep the codebase healthy and maintainable.

If you're working on a feature branch, you are not required to make new tests for code or have new tests pass. You may want to submit a failing test to demonstrate a bug but not have the time or know-how to fix it.

### Formatting
Making sure formatting, naming conventions, and file structure match the rest of the project is essential to keeping a healthy codebase. Use common sense when applying this to your contributions.

*I am not too picky about how the code is formatted inside methods.*

### Documentation
If you're changing the codebase, make sure any classes, methods, etc., are well documented, so it's easy to understand what your change is doing. This project uses [summary tags](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc) for documenting code structures.

### New Dependencies
If you need to include a new dependency into the project, mention that in the issue about your contribution. The dependency should also be compatible with the project's [current license](https://mit-license.org/).

## Before you Submit
Once you've made your changes, there are a few more steps to take before submitting a pull request.

1. Merge the up-to-date branch into your branch.
2. Make sure testing requirements for your branch are satisfied.

## You're good to go!
Once you submit your pull request, it'll be under review. During the review process, you will receive feedback on what to change. When everything looks good, the request will be merged.

### I hope to see new contributions from you which move the project forward in extraordinary ways.
