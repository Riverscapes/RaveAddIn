---
title: Web RAVE Vector Symbology
---

The following instructions are intended for riverscapes tool owners to use to define Web RAVE symbology. They instructions pertain to **vector** symbology only. Separate instructions will be provided for raster symbology.

A video of the entire workflow is included at the bottom of this page.

# A Quick Overview of the Process

Web RAVE uses symbology defined as [JSON](https://en.wikipedia.org/wiki/JSON), a text format common across the web. This is unlike QRAVE that uses QML files and ArcRAVE that uses layer files.

The JSON in question is a little complicated to write it by hand, so the following workflow  uses an online tool called MapBox to configure the symbology within a user interface and from here export the JSON needed for Web RAVE.

The first few steps involve uploading a sample dataset for the type of layer you want to symbolize and into MapBox so you can develop the symbology on a real set of data. When you are finished the data you use will be discarded. We just keep the symbology that you created.

# Before You Start

You should have the following available before you begin:

1. A data layer for which you want to define the symbology for Web RAVE.
1. An idea of how you want the layer in question to be symbolized. This will most likely be informed from symbology that has already been defined for QRAVE or ArcRAVE.
1. QGIS Desktop GIS software.
1. A free [MapBox](https://studio.mapbox.com) account.
1. A powerful text editor, such as [Visual Studio Code](https://code.visualstudio.com/).

# Prepare Your Data

1. Start QGIS and load the layer you want to symboloize into the map. If possible, use QRAVE to help you do this. In other words, download a project that contains the layer in question and add it to the current map document by double clicking it in the RAVE project tree.
1. Export the layer to a GeoJSON file:
    1. Right click on the layer in the QGIS table of contents and choose **Export** and then **Save Features As...**
    1. Make sure that GeoJSON is selected in the top dropdown.
    1. Specify a file path where the data will get stored.
    1. Choose WGS84 as the projection.
    1. Click **Save**
    1. MapBox seems to struggle with map projections defined in GeoJSON files, so open the file you just exported in a text editor and remove the line near the top starting with "crs". An example of the line to remove is shown below. Remember to save the file after.

```json
"crs": { "type": "name", "properties": { "name": "urn:ogc:def:crs:OGC:1.3:CRS84" } },
```

If you're layer is big (> 5Mb) then you might consider only exporting a subset of your data, or omitting some of the attribute fields. Remember to retain any features and fields that span the gammit of symbology you intend to implement. All you really need is one feature of each symbol type that you want in the final layer symbology. The easiest way to omit features is to use a "definition query" to filter which features are considered part of the layer in QGIS. Alternatively, if you know JSON you can just delete features from the GeoJSON file in the same step where you edit the file to remove the `crs` projection.

# Upload Your Data to MapBox

1. Login into MapBox Studio.
1. Click the **Styles** tab and then **create new style**. (If you don't see a styles tab then click on your account icon top right and choose **Studio** to be taken to the correct screen).
1. Choose Blank Template and then the **Create Blank Template** button at the bottom.
1. Click the word "Blank" in the top bar to make the name editable and provide a good name (this name is not important for Web RAVE, but is useful because this style is going to persist in your MapBox account and you might need to find it to tweak it in the future).
1. Click on the **Layers** tab.
1. Click the plus icon to add a layer.
1. Click the **Upload data** button in the source panel and upload the GeoJSON that you exported in the previous step. Large datasets will take some time to both upload and process the data. Also note that the free MapBox accounts has limits on the amount of data you can store in your account.

# Symbolize the Layer

1. Click the Source dropdown and select the data source that you just uploaded. You might have to scroll among all the default layers that MapBox provides.
1. It's helpful to have a basemap for context. Click the Source Drop down again, but this time pick one of the built-in MapBox satellite layers.
1. Now it's time to symbolize your layer. Some guidelines are below. 
    - You need to duplicate the layer and change the type to get things like labels. So, for example, you might have one mapbox layer for symbols (labels), one for line styling and one for polygon fill styling. All layers consume from the same datasource.
    - You can add copies of your datasource and convert them for things like labels, lines, polygons etc but you're not allowed to use anything else. Everything must derive from the geojson file you uploaded.
    - You can add support layers like satellite maps underneath to help you see what your map will look like but you need to make sure these don't end up in the final product below. More about that later.
    - Search for oneline tutorals or videos. MapBox is well supported.

# Export the Symbology

1. When you're happy with your symbology click the Share button in the top right of the screen.
1. In the screen that pops up, click the **Draft** top right.
1. Click the Download link at the bottom of the page to download a zip file.
1. Extract the zip file and open the `style.json` file in a text editor.
1. Select all the text from the square brace after `"Layers":` until the corresponding closing square brace. See video below for example.
1. Paste the text into a new text file.
1. Save the file into the WebRAVE folder in the Riverscapes XML git repository under the path [symbology\webrave](https://github.com/Riverscapes/RiverscapesXML/tree/master/Symbology). Use the same name as the [business logic symbology key]({{ site.baseurl }}/symbology.html#symbology-keys). The name is case sensitive and the file suffix must be `.json`.
1. Commit your changes and create a pull request, just as you would for QRAVE QML or ArcRAVE layer files.

The final symbology file will look something like this:

```json
[
	{
        "id": "bratreachgeometry-bv59z4",
        "type": "line",
        "source": "composite",
        "source-layer": "bratReachGeometry-bv59z4",
        "layout": {},
        "paint": {"line-color": "hsl(0, 100%, 44%)", "line-width": 10}
    },
    {
        "id": "bratreachgeometry-bv59z4 copy",
        "type": "symbol",
        "source": "composite",
        "source-layer": "bratReachGeometry-bv59z4",
        "layout": {"text-field": ["get", "NHDPlusID"]},
        "paint": {"text-color": "hsl(0, 0%, 100%)"}
    }
]
```

Note that your symbology will be live as soon as the pull request is approved and implemented.

# Video Demonstration

<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/vIFQJlEIgpc" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>
