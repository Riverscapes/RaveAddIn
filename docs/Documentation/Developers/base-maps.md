---
title: Base Maps
---

In addition to helping load riverscapes projects into the current ArcMap document, RAVE also supports loading base map layers.
Base maps are [Web Mapping Services](https://en.wikipedia.org/wiki/Web_Map_Service) (WMS) that can be consumed over the internet and displayed underneath your local map layers. This is a new feature in RAVE and its worth reviewing the limitations further down this page before proceeding.

# Enabling and Disabling Base Maps

Click on **Options** in the RAVE Toolbar under the Help menu. Check the box to enable base maps to appear in the RAVE project 
explorer and then select the "region" that you want to appear. Click OK and the selected base maps should now appear at the bottom of the RAVE project explorer.

![steps]({{site.baseurl}}/assets/images/base_maps_steps.png)

# Adding Base Maps to the Current Map Document

Right click on a base map item in the RAVE project explorer tree and choose **Add To Map**. The selected base map should appear at the bottom of the Table of Contents within the current map document.

![add]({{site.baseurl}}assets/images/add_base_map.png)

# Base Map Definition Files

The base maps available within RAVE are listed in plain text XML files called `BaseMaps.xml`. RAVE looks in two locations for this file in a specific order:

```
%APPDATA%\RAVE\BaseMaps.xml
<software_installation_folder>\BaseMaps.xml
```

The first location is not created by default. If you want to customize the base maps available in RAVE then create this file and follow the instructions below.

The second location describes the list of the base maps that is deployed with the RAVE softwware. This list is updated with each release of the RAVE software.

Within either of these files RAVE will load any "regions" and their corresponding Web Mapping Service base maps.

# Customizing Your Own Base Maps

These steps describe how to create your own `BaseMaps.xml` file
so that RAVE can make use of additional Web Mapping Services.

1. Create a text file at the following path. `%APPDATA%\RAVE\BaseMaps.xml`. See the [symbology page](symbology.html#where-is-my-appdata-folder) for how to locate your APPDATA folder. The `RAVE` folder might not exist.
1. List the Web Mapping Services that you want to use by writing the necessary XML. See below.
1. Open the Options form in the RAVE Help menu and select name of the region that you used in your custom BaseMaps.xml file.

# XML Syntax for Base Maps.xml

The sample below shows the fairly simple structure of the BaseMaps.xml file. Working with this XML file is demonstrated in the video at the bottom of this page.

The one and only root node must be called `BaseMaps`. It can contain multiple `Region` nodes that represent different geographic regions of the World for which web mapping services are maintained. Regions are intended to be a loose concept for grouping together base map layers for the same parts of the World. They can be repurposed if you have other types of Web Mapping Service collections.

Each `Region` can contain either `GroupLayer` or `Layer` tags. GroupLayer tags are merely to help with organization. They show up in the RAVE project explorer as folders and the ArcMap table of contents as group layers. 

The `Layer` tag represents a single Web Mapping Service. Each Layer tag must have a name attribute that contains the text that will be displayed in the RAVE project explorer, as well as a URL attribute that contains the end point for the Web Mapping Service. The Metadata child tags are optional and not used by RAVE at this time.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<BaseMaps>
  <Region name="United States">
    <GroupLayer name="USGS">
      <Layer name="Topo Base Map" url="https://basemap.nationalmap.gov:443/arcgis/services/USGSTopo/MapServer/WmsServer?">
        <Metadata>
          <Meta name="Description">USGS Top Map Base Layer</Meta>
        </Metadata>
      </Layer>
      <Layer name="Shaded Relief" url="https://basemap.nationalmap.gov:443/arcgis/services/USGSShadedReliefOnly/MapServer/WmsServer?">
        <Metadata>
          <Meta name="Description">USGS shaded relief</Meta>
        </Metadata>
      </Layer>
    </GroupLayer>
   </Region>
</BaseMaps>
```

# Limitations

The RAVE Base Maps feature is experimental and still undergoing testing. Web Mapping Services are one of many internet mapping protocols and we are still ironing out the kinks of how to document them in the BaseMaps.xml file so that they can be easily added to the map. For now, you should limit your base maps to WMS and avoid other types of internet mapping such as WTMS or WFS etc.

Some Web Mapping Services, like those available in New Zealand, require an API key as part of the URL. We intend to implement this feature in the near future.

Please [report all bugs and issues](https://github.com/Riverscapes/RaveAddIn/issues) with the base maps feature.

# Video Demonstration

<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/2SLKCW6M9ik" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>
