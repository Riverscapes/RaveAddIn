---
title: ArcRAVE Help
weight: 3
---

# Getting Started

# Default Project View

# Opening a Project

Launch ArcMap and make sure that the RAVE toolbar is visible. If it is not then right click in the empty toolbar area in the top right of ArcMap and choose "River Analysis Viewer and Explorer". This will open the toolbar which can then be docked anywhere within the ArcMap main window.

![toolbar]({{site.baseurl}}/assets/images/toolbar.png)

Click on the green plus icon to open an existing Riverscapes compatible project. Browse to and select the project file (`*.rs.xml`) then click "Open" to open the project. RAVE will read the project file, determine what type of Riverscapes project it represents, and then find the corresponding [business logic XML](business-logic.xml) file that specifies how to display the project within the RAVE project explorer. Once the project is loaded the project explorer will contain the project tree and you can start adding layers to the current ArcMap document by right clicking on them and choosing "Add To Map".

![add to map]({{site.baseurl}}/assets/images/add_to_map.png)

You can customize the symbology used when layers are added to the map by generating layer files. Read the [Symbology](symbology.html) for instructions on how to do this.

# Adding Layers to the Map


In addition to helping load riverscapes projects into the current ArcMap document, RAVE also supports loading base map layers.
Base maps are [Web Mapping Services](https://en.wikipedia.org/wiki/Web_Map_Service) (WMS) that can be consumed over the internet and displayed underneath your local map layers. This is a new feature in RAVE and its worth reviewing the limitations further down this page before proceeding.

# Base Maps

## Enabling and Disabling Base Maps

Click on **Options** in the RAVE Toolbar under the Help menu. Check the box to enable base maps to appear in the RAVE project 
explorer and then select the "region" that you want to appear. Click OK and the selected base maps should now appear at the bottom of the RAVE project explorer.

![steps]({{site.baseurl}}/assets/images/base_maps_steps.png)

## Adding Base Maps to the Current Map Document

Right click on a base map item in the RAVE project explorer tree and choose **Add To Map**. The selected base map should appear at the bottom of the Table of Contents within the current map document.

![add]({{site.baseurl}}assets/images/add_base_map.png)


# Project Views

# The Project Explorer Panel