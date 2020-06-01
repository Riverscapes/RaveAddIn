---
title: Business logic XML
weight: 96
---

The purpose of the business logic XML is to translate Riverscpaes projects and determine how they should be displayed in the RAVE project explorer. In other words, the Riverscapes project file defines **what** layers exist within a project, while the business logic file defines **how** these layers should be organized within the project explorer tree and be dispalyed when added to map..

RAVE comes pre-loaded with business logic for Riverscape Consortium [registered models and tools](https://riverscapes.xyz/Tools/Technical_Reference/Documentation_Standards/Riverscapes_Projects/Program/).  Typically there is one business logic XML file for each type of Riverscapes project (e.g. BRAT, VBET, GCD etc). But it's also possible to have one or more custom business logic XML files for a single project type. This allows users to have different views of a single project type. Perhaps there's one during development and another for reviewing projects with clients etc. If you need this ability see the [custom business logic](#custom-business-logic) and [search folders](#where-does-rave-look-for-business-logic-xml-files) sections below.

![business logic]({{site.baseurl}}/assets/images/business_logic.png)

## Where does RAVE Look for Business Logic XML Files?

When RAVE attempts to load a Riverscapes project it looks in three locations to find an appropraite business logic XML file. The search order it uses is:
1. User-loaded from **Customize Project Hiearchy** command (Priority 1)
2. In the root directory of the current project (Priority 2)
3. Local user default over-ride for that project type  (Priority 3)
4. Defualt what ships with that version of RAVE (Priority 4)

RAVE uses the first business logic file that it finds that contains the same project type specified at the top of the business logic file. Note that Priorities 2 & 3 represent user customization and Priority 1 is where the user can manually load business logic with the **Customize Project Hiearchy** command (see below). See the [symbology page](symbology.html#where-is-my-appdata-folder) for how to locate your APPDATA folder.


|Search Folder|Example| Priority|
|---|---|---|
|In root directory of current riverscapes project file.|`D:\MyProjects\dem.lyr`| Priority 2|
|`%APPDATA%\RAVE\XML`|`%APPDATA%\RAVE\XML\brat.xml`|Priority 3|
|`<software_installation_folder>\XML`|*Hidden folder*|Priority 4 **DEFAULT**|

[![refresh]({{site.baseurl}}/assets/images/RAVE-Order_650wl.png)]({{site.baseurl}}/assets/images/RAVE-Order_Full.png)


## Customizing Business Logic & Refreshing the Project Explorer

You can change the business logic XML file while ArcMap and RAVE are in use, providing that the business logic file in question is either adjacent to the Riverscapes project file or in the APPDATA folder.

1. Locate the business logic file you wish to use and open it in any text editor.
1. Make the desired changes to the business logic XML.
1. Save the business logic XML file (to project for Priority 2, or to `%APPDATA%\RAVE\XML\` for Priority 3).
1. Right click on the project node in the RAVE project explorer and choose "Refresh".

![refresh]({{site.baseurl}}/assets/images/refresh.png)

## Custom Business Logic

You can apply a business logic file stored in a location on your computer other than the search folders described above by choosing the "Customize Project Hierarchy" option after right clicking on the project in the RAVE project explorer.

![custom]({{site.baseurl}}/assets/images/custom.png)

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

Repeaters can contain `<Node>` types. The `<Node>` inside a will be applied to each repeated element found inside that repeater's xpath results.

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

# Sharing your Business Logic with Someone Else

## Suggesting a New Default Business Logic for RAVE for Existing or New Riverscape Project Type

