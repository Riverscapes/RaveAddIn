---
title: Getting Started
weight: 1
---

## Prerequisities

Before you can use the RAVE AddIn for ArcGIS you will need the following:

1. ArcGIS 10.4 or above. RAVE does not work with ArcPro.
1. One or more Riverscapes compatible projects (with *.rs.xml)
1. A [business logic XML file](business-logic.html) that defines how to load each project type into the RAVE project explorer.

## RAVE Installation

Follow the steps on the [installation](install.html) page to get the latest RAVE AddIn for ArcGIS.

## Opening a RAVE Project

Launch ArcMap and make sure that the RAVE toolbar is visible. If it is not then right click in the empty toolbar area in the top right of ArcMap and choose "River Analysis Viewer and Explorer". This will open the toolbar which can then be docked anywhere you like within ArcMap.

![toolbar]({{site.baseurl}}/assets/images/toolbar.png)

Click on the plus icon to Open an existing Riverscapes compatible project. Browse to the project file (`*.rs.xml`) and click OK to open the project. RAVE will read the project file, determine what type of Riverscapes project it defines, and then find the corresponding [business logic XML](business-logic.xml) file that specifies how to load the project into the RAVE project explorer. Once the project is loaded the explorer will contain the project tree according and you can start adding layers to the current ArcMap document by right clicking on them and choosing "Add To Map".

![add to map]({{site.baseurl}}/assets/images/add_to_map.png)
