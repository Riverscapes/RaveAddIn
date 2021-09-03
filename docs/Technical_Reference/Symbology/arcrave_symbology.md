---
title: ArcRAVE Symbology
weight: 3
---

ArcRAVE stores symbology in proprietary ESRI layer files. Each type of RAVE dataset has its own layer file.

ESRI Layer files contain two bits of information. First they possess a path to a specific GIS dataset on your computer and second they store the definition of how the dataset should be symbolized. RAVE only uses the latter and ignores the path to the dataset itself. In this way RAVE can apply the symbology stored in a single layer file to many layers of the same type. See the [video demonstration](#video-demonstration) below to see for how all of this works.

#### ArcRAVE - How To Create a Layer (`*.lyr`) File

1. Add the layer in question to the current ArcMap document. You can do this using RAVE or using the typical ArcMap features to Add Data.
2. Use ArcMap to symbolize the layer how you want it to appear for RAVE layers. Remember that depending on where you place this layer file (see below) this symbology might be applied more than just the layer in question. Configure the symbology accordingly. For example, if you are configuring a percent slope layer but the current layer only contains slopes up to 20%, be sure to extend the symbology classification all the way up to 90% in case this symbology is used for layers with a different range.
3. Right click on the layer in the ArcMap table of contents and choose `Save As Layer File...` Be sure to save it in the right location according to how you want RAVE to use the layer file (see previous section).

![layer file]({{site.baseurl}}/assets/images/layer_file.png)

#### Video Demonstration

<div class="responsive-embed">
<iframe width="560" height="315" src="https://www.youtube.com/embed/msaGPVzmnxk" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>


