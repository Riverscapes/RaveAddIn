import { GatsbyConfig } from 'gatsby'

module.exports = {
  // TODO: You need pathPrefix if you're hosting GitHub Pages at a Project Pages or if your
  // site will live at a subdirectory like https://example.com/mypathprefix/.
  pathPrefix: '/',
  siteMetadata: {
    title: `Riverscapes Viewer`,
    author: {
      name: `North Arrow Research`,
    },
    // TODO: Just leave `helpWidgetId` as an empty string ('') if you don't want the riverscapes help widget in the footer
    helpWidgetId: '153000000178',
    description: ``,
    siteUrl: `https://viewer.riverscapes.net`,
    social: {
      twitter: `RiverscapesC`,
    },
    menuLinks: [
      {
        title: 'About',
        url: '/about',
        items: [
          {
            title: 'About Riverscapes Viewer',
            url: '/about',
          },
          {
            title: 'Acknowledgements',
            url: '/about/acknowledgements',
          },
          {
            title: 'Questions, Feature Requests & Bugs',
            url: '/about/known-bugs',
          },
          {
            title: 'License & Source Code',
            url: '/about/license',
          },
        ],
      },
      {
        title: 'Deploy',
        url: '/deploy/install-qgis',
        items: [
          {
            title: 'Riverscapes Viewer for the Web',
            url: '/Deploy/install-web',
          },
          {
            title: 'Riverscapes Viewer for QGIS',
            url: '/Deploy/install-qgis',
          },
          {
            title: 'Riverscapes Viewer for ArcGIS Pro',
            url: '/Deploy/install-arcpro',
          },
        ],
      },
      {
        title: 'Software Help',
        url: '/software-help',
        items: [
          {
            title: 'Riverscapes Web Viewer',
            url: '/software-help/help-web',
          },
          {
            title: 'Riverscapes Viewer for QGIS',
            url: '/software-help/help-qgis',
          },
          {
            title: 'Riversccapes Viewer for ArcGIS Pro',
            url: '/software-help/help-arcpro',
          },
        ],
      },
      {
        title: 'Technical Reference',
        url: '/technical-reference',
        items: [
          {
            title: 'Base Maps',
            url: '/technical-reference/base-maps',
          },
          {
            title: 'Business Logic',
            url: '/technical-reference/business-logic',
          },
          {
            title: 'Project Views',
            url: '/technical-reference/project-views',
          },
          {
            title: 'Symbology',
            url: '/technical-reference/symbology',
          },
          {
            title: 'ArcViewer Symbology',
            url: '/technical-reference/symbology/symbology-arc',
          },
          {
            title: 'QViewer Symbology',
            url: '/technical-reference/symbology/symbology-qgis',
          },
          {
            title: 'Web Vector Symbology',
            url: '/technical-reference/symbology/symbology-web-vectors',
          },
          {
            title: 'Web Raster Symbology',
            url: '/technical-reference/symbology/symbology-web-rasters',
          }
        ]
      }
    ],
  },
  plugins: [
    {
      resolve: '@riverscapes/gatsby-theme',
      options: {
        contentPath: `${__dirname}/content/page`,
        manifest: {
          name: `Riverscapes Gatsby Template Site`,
          short_name: `RiverscapesTemplate`,
          // TODO: You need to change this to your site's URL. This should match the `pathPrefix` above.
          start_url: `/`,
          iconUrl: `./static/viewer_favicon.png`,
        },
      },
    },
  ],
} as GatsbyConfig
