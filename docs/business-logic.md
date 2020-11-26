---
title: Business logic XML
weight: 96
---

The purpose of the business logic XML is to translate Riverscpaes projects and determine how they should be displayed in the RAVE project explorer. In other words, the Riverscapes project file defines **what** layers exist within a project, while the business logic file defines **how** these layers should be organized within the project explorer tree and be displayed when added to map..

RAVE comes pre-loaded with business logic for Riverscape Consortium [registered models and tools](https://riverscapes.xyz/Tools/Technical_Reference/Documentation_Standards/Riverscapes_Projects/Program/).  Typically there is one business logic XML file for each type of Riverscapes project (e.g. BRAT, VBET, GCD, etc.). But it's also possible to have one or more custom business logic XML files for a single project type. This allows users to have different views of a single project type. Perhaps there's one during development and another for reviewing projects with clients etc.. If you need this ability see the [custom business logic](#custom-business-logic) and [search folders](#where-does-rave-look-for-business-logic-xml-files) sections below.

![business logic]({{site.baseurl}}/assets/images/business_logic.png)

## Where does RAVE Look for Business Logic XML Files?

When RAVE attempts to load a Riverscapes project it looks in three locations to find an appropriate business logic XML file. The search order it uses is:
1. User-loaded from **Customize Project Hierarchy** command (Priority 1)
2. In the root directory of the current project (Priority 2)
3. Local user default over-ride for that project type  (Priority 3)
4. Default what ships with that version of RAVE (Priority 4) - *Note that you should always make sure you have the [latest verson of RAVE](https://github.com/Riverscapes/RaveAddIn/releases/latest) as default symbology and business logic updates are published in new releases.*

RAVE uses the first business logic file that it finds that contains the same project type specified at the top of the business logic file. Note that Priorities 2 & 3 represent user customization and Priority 1 is where the user can manually load business logic with the **Customize Project Hierarchy** command (see below). See the [symbology page](symbology.html#where-is-my-appdata-folder) for how to locate your APPDATA folder.


|Search Folder|Example| Priority|
|---|---|---|
|In root directory of current riverscapes project file.|`D:\MyProjects\dem.lyr`| Priority 2|
|`%APPDATA%\RAVE\XML`|`%APPDATA%\RAVE\XML\brat.xml`|Priority 3|
|`<software_installation_folder>\XML`|*Hidden folder*|Priority 4 **DEFAULT**|

[![refresh]({{site.baseurl}}/assets/images/RAVE-Order_650wl.png)]({{site.baseurl}}/assets/images/RAVE-Order_Full.png)

## Loading Custom Business Logic

You can load a custom business logic file stored in a location on your computer other than the search folders described above by choosing the **Customize Project Hierarchy** option after right clicking on the project in the RAVE project explorer.

![custom]({{site.baseurl}}/assets/images/custom.png)

## Customizing Business Logic & Refreshing the Project Explorer

You can customize the business logic XML file while ArcMap and RAVE are in use, providing that the business logic file in question is either adjacent to the Riverscapes project file or in the APPDATA folder. **WARNING** - the business logic is written in XML and is fiddly (see these instructions).  Make small changes locally incrementally (either in Priority 2 or 3 locations on disc), and frequently referesh and test within RAVE.

1. Locate the business logic file you wish to use and open it in any text editor (we strongly recommend following [these instructions](https://riverscapes.xyz/Tools/Technical_Reference/Documentation_Standards/Riverscapes_Projects/xml_validation.html) for editing and authoring riverscapes XML in Visual Studio Code so that it validates your XML).
1. Make the desired changes to the business logic XML.
1. Save the business logic XML file (to project for Priority 2, or to `%APPDATA%\RAVE\XML\` for Priority 3).
1. Right click on the project node in the RAVE project explorer and choose "Refresh".

![refresh]({{site.baseurl}}/assets/images/refresh.png)



## Business logic Definition

Here is an example of a business logic file for VBET projects:

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:noNamespaceSchemaLocation="XSD/project_explorer.xsd">
  <Name>VBET</Name>
  <ProjectType>VBET</ProjectType>
  <Node xpathlabel="Name">
    <Children>
      <Repeater label="Realizations" xpath="Realizations/VBET">
        <!--This is a template for how each realization should render-->
        <Node xpathlabel="Name">
          <Children collapsed="false">
            <Node label="Inputs" xpath="Inputs">
              <Children>
                <Node label="Topography" xpath="Topography">
                  <Children>
                    <Node xpathlabel="Name" xpath="DEM" type="raster" symbology="DEM"/>
                    <Node xpathlabel="Name" xpath="Flow" type="raster" symbology="Flow"/>
                    <Node xpathlabel="Name" xpath="Slope" type="raster" symbology="SlopePer"/>
                  </Children>
                </Node>
                <Repeater label="Network Buffers" xpath="DrainageNetworks/Network/Buffers/Buffer">
                  <Node xpathlabel="Name" type="vector" symbology="DEMO_singlefill"/>
                </Repeater>
              </Children>
            </Node>
            <Node label="Analyses"/>
          </Children>
        </Node>
      </Repeater>
    </Children>
  </Node>
</Project>
```

By layering the various building blocks together you should be able to accomplish a wide variety of tree structures.

### `<Project>`

### `<Repeater>`

Repeaters can contain `<Node>` types. The `<Node>` inside a will be applied to each repeated element found inside that repeater's `xpath` results.

Repeaters have one attribute: `xpath` that you can use to specify the object that is repeated in the project xml.

### `<Children>`

Children can contain `<Node>` and `<Repeater>` types. They can only be used inside other `<Node>` tags.

They have one optional attribute: `collapsed="true/false"` which determines if this group is collapsed by default upon load.

### `<Node>`

#### Tags

* `<TileService>`: (optional) If your node is a tile service (`type="tile"`) then put its url here
* `<Children>`: This node can have sub-nodes

#### Attributes

Choose one of:
* `label`: The literal string you want to assign to this node as a label
* `xpathlabel`: if you don't use `label` and you want to look up your node label from the xml file you can do it this way

The following are optional:
* `type`: If you want your node to be added to the map you need to specify what its type is. See the "Node Data Types" section below for allowed values.
* `symbology`: If this is a map layer you can specify what symbology to use. This must match the `NAME` variable from the python symbology plugin in `RiverscapesToolbar/symbology/plugins/myplygin.py`
* `xpath`: Not to be confused with `xpathlabel`. This xpath sets the context for this node. Children nodes will inherit it.


### Node Data Types

Nodes can have different data types which affect how (if) the node loads into the map.

* `raster`: tif files
* `vector`: shp files
* `tile`: A url
* `file`: catch-all for any other kind of file. Right now this will just open up finder/explorer to this location.

## Project Views

The business logic file can define groups of layers that can be loaded into the map as a "view". These are useful if there are common or default ways of viewing the contents of a project. Each business logic file can define multiple views, but only one can be designated the "default view". The default viewis loaded automatically into the current ArcMap document when the project is loaded (if that option is selected in the RAVE options).

The following business logic example for BRAT renders as the user interface tree in RAVE shown below.

```xml
  <Views default="BRAT Outputs">
    <View name="BRAT Outputs">
      <Layer id="hillshade" />
      <Layer id="capacity"/>
    </View>
    <View name="BRAT Vegetation">
      <Layer id ="hillshade"/>
      <Layer id="vegetation"/>
    </View>
  </Views>
  ```

![project view]({{ site.baseurl}}/assets/images/project_views/project_view_tree.png)

### `ProjectViews`

The `ProjectViews` node should appear inside the top level `Project` node. It's only attribute is `default` that identifies the name of the project view that is considered the default project view.

### `View`

Each view has a `name` attribute that contains the display name that will be used for the view in the user interface.

### `Layer`

Each `Layer` is a reference to a `Node` in the project nodes within the project. Layers can refer to any node, include group nodes that have children. `Layer` tags have an `id` attribute that corresponds to the `id` tag of the project node to which it refers. The `visible` attribute is not yet functional but should control whether the layer is turned on for display when it is added to the map.

### Video Demonstration

<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/JvPb4ZoO4qU" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>
------

# Sharing your Business Logic with Someone Else

If you have gone to the extent of customizing and/or authoring your business logic, you have three main options for sharing it with others (in order of complexity).

## Option 1:  Package up with a Riverscapes Project 
Most RAVE users will have no idea what business logic is. If you have customized business logic for a specific Riverscapes Project, the easeist way to share it with someone else is to ship them your riverscapes project with the customized buisiness logic in the root of the project directory (as well as the `*.lyr` layer files the `symbology` keys point to). This video illustrates an example of this:

<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/UOYYiOILwYY" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

## Option 2:  Update their Local Deafults

For an advanced RAVE user, you can send them the `*.xml` business logic file and corresponding  `*.lyr` layer files the `symbology` keys point to, and then instruct them as follows:
1. Place the `*.xml` business logic file in the `%APPDATA%\RAVE\XML`folder (note they may need to create this folder if it does not exist).
2. Place all the `*.lyr` layer file(s) in the `%APPDATA%\RAVE\Symbology\[NAME]`folder where `[NAME]` is the same case and name as the project type (e.g. for `brat.xml` business logic this would be a subfolder of name `brat`. Note, they may need to create this `Sybmology` folder if it does not exist.
3. Load a riverscapes project, or if a project is already loaded **Refersh Project Hiearchary** tree. 

This video illustrates how this works:
<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/L7JGVlAaua0" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

## Option 3: Suggesting a New Default Business Logic for RAVE for Existing or New Riverscape Project Type

If you would like the Riverscapes Consortium development team to consider your business logic updates and symbology updates for the next release of RAVE, propose a [pull request  <i class="fa fa-github"></i>](https://github.com/Riverscapes/RaveAddIn/pulls) to the [RaveAddIn repository](https://github.com/Riverscapes/RaveAddIn) . If you are part of the Riverscapes Consortium development team, you can do this by making a local Branch, making your commits and pushes to that branch (name it something logical like `ProposedSybmologyMyModel`) and then log a pull request and ask for a review. If you are not part of the development team, you can fork the repo, make your changes on your own repo and then submit a pull request.  This request will be reviewed by the development team and they may ask for fixes after testing it. If accepted, this will be merged into the main branch and reflected in the next release. 