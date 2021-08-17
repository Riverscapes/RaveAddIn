---
title: Open a Project
weight: 3
---

Launch ArcMap and make sure that the RAVE toolbar is visible. If it is not then right click in the empty toolbar area in the top right of ArcMap and choose "River Analysis Viewer and Explorer". This will open the toolbar which can then be docked anywhere within the ArcMap main window.

![toolbar]({{site.baseurl}}/assets/images/toolbar.png)

Click on the green plus icon to open an existing Riverscapes compatible project. Browse to and select the project file (`*.rs.xml`) then click "Open" to open the project. RAVE will read the project file, determine what type of Riverscapes project it represents, and then find the corresponding [business logic XML](business-logic.xml) file that specifies how to display the project within the RAVE project explorer. Once the project is loaded the project explorer will contain the project tree and you can start adding layers to the current ArcMap document by right clicking on them and choosing "Add To Map".

![add to map]({{site.baseurl}}/assets/images/add_to_map.png)

You can customize the symbology used when layers are added to the map by generating layer files. Read the [Symbology](symbology.html) for instructions on how to do this.