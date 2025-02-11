# Contributing to This Project

Thank you for considering contributing to this project! We appreciate your help and effort in improving this project.

## How to Contribute

Before getting started and setup with contributing, you'll want to look at and choose an issue to work on. Here is the workflow you want to work from:

1. Search through issues
2. Find issue you want to work on
3. Check if someone else has already worked on and made a pull request on said issue
4. (Optional) Double check pull requests for someone who has worked on the pull request

If you have gotten that far, then you can go ahead and work on the issue. Below are more detailed instructions based on the basic workflow above.

You can find open issue [here](https://github.com/QalaTech/qala-cli/issues).

Once you've found an issue you want to work on, take a look at the issue to see if anyone else has made a pull request for this issue yet.

You can tell if someone has correctly referenced and worked on an issue if in the issue you find some text saying, the following:

>  This was referenced on ____

where that `____` is the date and below it is the pull request of another individual working on that issue.

To be extra sure no one has worked on it, you can take a look at the pull requests as well to see if anyone has made a similar pull request.

If you haven't found an open issue with your possible contribution, please open a new one [here](https://github.com/QalaTech/qala-cli/issues) and wait for our team to approve.

If you've gotten this far, then you can continue on with the next section to work on your first pull request and contribution to our repository.

If you are new to Git and GitHub, it is advisable that you go through [GitHub For Beginners](http://readwrite.com/2013/09/30/understanding-github-a-journey-for-beginners-part-1/) before moving forward.

### 1. Fork the Repository
Fork the repository to your GitHub account by clicking the `Fork` button at the top right of the repository page.

Here's a [Help Guide to Fork a Repository](https://help.github.com/en/articles/fork-a-repo/).

### 2. Clone the Repository
Clone your forked repository to your local machine:
```sh
git clone https://github.com/<your-username>/qala-cli.git
cd qala-cli
```

Here's a [Help Guide to Clone a Repository](https://help.github.com/en/articles/cloning-a-repository).

### 3. Create a Branch
#### 3.1 New Feature

Create a branch from master:

```sh
# Assuming you are in master
git checkout -b feature/<issue-id>
```

#### 3.2 Bug fixing

Create a branch from the latest release branch:

```sh
git checkout release/<latest-version>
git checkout -b hotfix/<issue-id>
```


### 4. Make Your Changes
Make the necessary changes on your favorite IDE and test them before committing.

### 5. Commit Your Changes
Commit your changes with a meaningful commit message and the issue Id:
```sh
git add .
git commit -m "<issue-id>: Add meaningful commit message"
```

### 6. Push to Your Fork
Push your branch to your forked repository:
```sh
git push origin <your-branch-name>
```

### 7. Open a Pull Request
Go to the original repository and click on `New Pull Request`. Select your branch and submit the pull request with a meaningful description.

Here's a [What is a Pull Request?](https://yangsu.github.io/pull-request-tutorial/) information.

Be sure to make the `PR`to the branch that you originally branched:
- If new feature, from `master`
- if bug fixing. from `release/<latest-version>`

## Code of Conduct
By contributing, you agree to follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## Issues and Discussions
If you find a bug, want to request a feature, or discuss an idea, please check the [issues](https://github.com/qala-cli/issues) section before creating a new one.

## Coding Guidelines
- Follow the project coding style.
- Write meaningful commit messages.
- Ensure your code is well-documented.
- Run tests before submitting a pull request.

## License
By contributing, you agree that your contributions will be licensed under the same license as the project.

Thank you for contributing!

