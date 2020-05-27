# bucharest

//TODO: Update information to be correct for this project

Using Unity Version 2019.3.14f1

--Git Standards--

Basic Etiquette
Since the entire team is working in a repository, all members must be mindful of each other when pushing changes. Git and command line seem scary at first, but once you get the hang of it it’s not that bad. Since this project is entirely remote, it is extremely important that we all follow the rules outlined below.

Communication is very important when using Git as a team. Make sure that you let the team know when you are merging branches, and speak up if you run into a merge conflict. This will help in not overwriting someone else’s work. If you feel that someone else’s work should be changed, make sure that you ask them first. Do not change other people’s code without asking.
Git Master(s)
	A Git Master is a member of the team that is well-versed and comfortable with using Git in a team setting. There can be more than one Git Master on a team. The Git Master is responsible for managing and maintaining the team’s repository. For this project, the Git Master(s) will handle creating and merging branches, in order to minimize merge conflicts.

Git Masters for this project: Ethan Heil, Henry Chronowski
Overview of Branches
Master Branch - Branch meant for the finished/polished product. Do not work on the master branch!
Dev Branch - Work in progress branch. This branch is where all of the unstable code and unfinished assets go. Artists can push assets to this branch without issue or needing to worry about merging branches. Designers and programmers should not touch code while in this branch!
Feature Branches - Branches created off of the dev branch that are meant for specific features. A Feature Branch should relate to a user story. These branches are mostly for the programmers or designers to mess around with game features. If you need a new feature branch to be created, let a Git Master know and they will create one if necessary. Keep pushing to these branches while a feature is still in progress, and make sure that you notify the other programmers/designers in Mattermost when you are going to push and pull from a branch. Notifying others when you push and pull will help to avoid merge conflicts. It also lets everyone else on the team know what you are working on, and what kind of progress has been made. 

Once a feature is complete and you are ready to merge, let a Git Master know (preferably through Mattermost). The Git Master will then handle merging that feature branch into the Dev branch.  


----------PIPELINE----------

Updating Your Branch
**Before doing anything on any branch or when starting a new work session, make sure that your working tree is clear and your branch is up to date.**

	Follow these steps to update your current branch:
	
	1. git status (Checks for any changes on your local machine)
	2. git fetch (Checks for any changes in remote repository)
	3. Notify the team in Discord that you are pulling changes from a specific branch.
	4. git pull (If fetch shows changes, pull the changes to your local machine)
	
Basic workflow while on any branch
This is the workflow that everyone will follow while on their own branch. Merging branches will be handled by the Git Master.
General Workflow
**Important note: You do not need to include <> in any of the following commands, they are just being used to differentiate the command from a file/folder name**

git status (Checks for any changes on your local machine)
git add <file/folder name> (Stages files for commit)
 	Optional add commands:
git add -u (Stages all modified files for commit, will not stage newly created files)
git add <folder name> (Stages everything in that folder)
git commit -m 'message' (Commits files to be pushed, add a commit message so you know what was worked on)
		Example commit messages:
'Adding README.txt'
'Adding move function to player.cs'
'Adding playerSprite.png to Assets folder'
Notify the team that you are pushing to your branch.
git push (Pushes files to the remote repository ON YOUR CURRENT BRANCH) 


Git Master Workflow
Branch Naming Conventions
All feature branches should follow this naming convention: feature_featureName

Example of creating a feature branch for player movement:
git checkout -b feature_playerMovement
Creating a new branch
git checkout -b <branch name>    (Creates a new branch and switches to it)
git push --set-upstream origin <branch name> (Pushes branch to the remote repository)
Deleting a branch
git branch -d <branch name>
Swapping to a different branch	
git checkout <branch name>
Merging branches
**Merging branches should only be done once a feature is complete unless the programmers are unified in awareness and agreement.**

Make sure you are on the feature branch that you want to merge
Follow steps 1-5 in the General Workflow
git checkout <branch name>    (Switches to specified branch, most likely will be dev)
Follow steps 1-4 in updating branches
git merge <branch you want to merge>
git status
git push

Example merging test branch into dev
(On test branch)
git status
If changes exist:
git add <file/folder name>
git commit -m 'commit message'
git push
If no changes/after committing changes:
git checkout dev    (Switch to dev branch)

(On dev branch)
git status
git fetch
git pull
git merge test (Merge test into dev)
git status
git push
	
**If you run into merge conflicts let a Git Master know!**

Tagging
A tag in Git for this project is a pointer to a specific commit. We will use tags for testing builds and the sprint deliverable each week. This will make it easy to refer back to different stages of the project if needed.

To create a tag on a commit, use the git tag command with one or both of the following options.
Use the -a <tag_name> option to add a name to a tag
Use the -m <tag_name> option to add a message to a tag
Using no option with the tag name will create a lightweight pointer to a commit

Follow this string of options with the commit ID of your current commit. To find a specific commit ID, use the git log command and copy and paste the desired ID (Only copy the hex number). 

To push the tag to the remote repository, use the git push origin tag <tag_name> command.

To list all of the tags that have been created, use the git tag command.
Artist Pipeline
Asset Location
All art assets will go into the ArtAssets folder, and not directly into the Unity Assets folder. The ArtAssets folder will be located in the project folder (..\sp20-egd220-project03-section06-team02\FreshAssets). Programmers or designers will be responsible for moving art assets into the Unity project and implementing them in the game. 

Pushing Assets in Git
Make sure you are on the dev branch
git checkout dev
git status
git fetch
git pull (if fetch finds changes)
git status 
git add <file/folder name>
git status
git commit -m 'commit message'
git status
git push
