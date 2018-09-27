---
title: Open a Project
---

Launch ArcMap and make sure that the RAVE toolbar is visible. If it is not then right click in the empty toolbar area in the top right of ArcMap and choose "River Analysis Viewer and Explorer". This will open the toolbar which can then be docked anywhere you like within ArcMap.

![toolbar]({{site.baseurl}}/assets/images/toolbar.png)

Click on the green plus icon to open an existing Riverscapes compatible project. Browse to and select the project file (`*.rs.xml`) then click OK to open the project. RAVE will read the project file, determine what type of Riverscapes project it defines, and then find the corresponding [business logic XML](business-logic.xml) file that specifies how to load the project into the RAVE project explorer. Once the project is loaded the project explorer will contain the project tree and you can start adding layers to the current ArcMap document by right clicking on them and choosing "Add To Map".

![add to map]({{site.baseurl}}/assets/images/add_to_map.png)
