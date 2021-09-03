---
title: RAVE
---

The Riverscapes Analysis Viewer and Explorer (RAVE) helps you make maps of rivers. RAVE speeds up the process of adding data related to rivers into your preferred GIS with meaningful layer order and symbology.

There are three versions of RAVE that all work essentially the same way. You start with a [riverscapes project](https://riverscapes.xyz/Tools/Technical_Reference/Documentation_Standards/Riverscapes_Projects/) that contains a collection of data layers related to rivers. You open the project in the RAVE project explorer that shows all the layers, displayed with meaningful names and icons. Clicking on a layer adds it to the current map in a carefulluy designed order with predefined symbology tailored to the layer in question.

There are three separate versions of RAVE depending on which GIS you prefer:


<div class="row">
    <div class="columns medium-4 small-12">
        <div class="card">
            <div class="card-section">
                <h4>WebRAVE</h4>

                No GIS needed! View riverscapes projects online in a browser.
<br />
<br />
                <div align="center">
                <a class="button" href="Download/install_webrave.html">Learn More</a>
                </div>
            </div>
        </div>     
    </div>
    <div class="columns medium-4 small-12">
        <div class="card">
            <div class="card-section">
                <h4>QRAVE for QGIS</h4>

                For desktop GIS users with <a href="https://qgis.org">QGIS</a> 3.16.10.

                <br />
<br />
                <div align="center">
                <a class="button" href="Download/install_qrave.html">Learn More</a>
                </div>
            </div>
        </div>     
    </div>
    <div class="columns medium-4 small-12">
        <div class="card">
            <div class="card-section">
                <h4>ArcRAVE for ArcGIS</h4>
                
                For desktop GIS users with ArcIGS 10.6.1 or higher.

                <br />
<br />
                <div align="center">
                <a class="button" href="Download/install_arcrave.html">Learn More</a>
                </div>
            </div>
        </div>     
    </div>
</div>

# Why RAVE?

River practitioners use lots of disparate geospatial data and need the ability to visualize it quickly.

However, simply adding a dataset to the current map document in desktop GIS can be frustrating for following reasons:

|GIS Challenge|RAVE Solution|
|---|---|
|Layers are added with default symbology (grey scale for rasters and a random color for vector layers). Apply appropriate symbolology to each layer is trivial but time consuming.|RAVE ships with a library of carefully curated symbology for each data type. Layers are always added to the map with consistent and appropriate symbology.|
|Layers are added to the top of the table of contents without contextual layer groups, requiring the user to adjust layers up and down and construct appropriate groups.|RAVE stores a logical tree of how layers should be organized for each type of project. Users can browse this tree and then add layers to the map, confident that RAVE will reconstruct the project tree in the table of contents.|
Layers are added to the table of contents with the dataset name by default.|RAVE ships with meainingful names for each layer that it uses when adding any layer to the map|
|Project data can be specific and lack the overall context required for interpretation.|RAVE makes available several free, public web mapping services making it easy to add aerial imagery and topographic base map layers with a single click.|

