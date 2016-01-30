Jake is the pivotal member of this team.

use `git checkout -b "mybranchname"` to create your own branch.
also `git push -u origin "mybranchname"`  to push your branch to github.

To commit your local changes
`git add <file>`
`git commit -m "message"`

To push those changes
`git push`

To merge your changes into master assuming you've pushed all of your changes to your own branch
`git checkout master`
`git pull`
`git merge <my local branch>`

At this point we may have merge conflicts
We will resolve all of them and then commit changes
`git commit`

Now we push those changes back up to master
`git push`

And switch back to our branch
`git checkout <local branch>`
