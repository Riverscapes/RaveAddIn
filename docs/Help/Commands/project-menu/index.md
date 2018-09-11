---
title: Project Menu
weight: 1
---

The main entry point to begin using the RAVE Software is the `Project` Menu:

![Menu_Project]({{ site.baseurl }}/assets/images/CommandRefs/01_Project/ProjectMenu_AddIn.png)


In the RAVE Software, all change detection analyses that you do are performed within a specific project. A [GCD project]({{ site.baseurl }}/Help/Concepts/project) consists of three primary elements:

1. A folder (fixed path) on your file system where the project and its contents exist.
2. A `*.rs.xml` [Project File]({{ site.baseurl }}/Help/Concepts/project/-gcd-files)
3. All the analysis input and output files (in `Inputs` and `Analyses` subfolders of project folder)

From the`Project` Menu, you can create a `New GCD Projects`, `Open...` existing projects and `Close `the current project. You can always tell if a project is open because it will be listed by its name in the GCD [Project Explorer]({{ site.baseurl }}/Help/Commands/gcd-project-explorer/):

![GCD6_GCDName]({{ site.baseurl }}/assets/images/GCD6_GCDName.png)

#### Commands

- [Open Project]({{ site.baseurl }}/Help/Commands/project-menu/open-project)
- [Close Project]({{ site.baseurl }}/Help/Commands/project-menu/close-project)

In addition to the above commands, the [Project Explorer]({{ site.baseurl }}/gcd-command-reference/project-explorer) can be accessed from the Project Menu, the Project Properties can be reviewed and the description edited, and you can [Browse Project Folder]({{ site.baseurl }}/gcd-command-reference/gcd-project-explorer/project-context-menu/iii-explore-gcd-project-folder).
