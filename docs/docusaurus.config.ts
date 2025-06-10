import { themes as prismThemes } from 'prism-react-renderer'
import type { Config } from '@docusaurus/types'
import type * as Preset from '@docusaurus/preset-classic'

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
  title: 'Riverscapes Viewer', // Site title displayed in the browser tab
  tagline: 'Riverscapes Viewer', // Short description shown in meta tags
  favicon: 'images/viewer_favicon.png', // Path to site favicon

  future: {
    v4: true, // Enables compatibility with upcoming Docusaurus v4 features
  },

  url: 'https://viewer.riverscapes.net', // The base URL of your site (no trailing slash)
  baseUrl: '/', // The sub-path where your site is served (used in GitHub Pages)

  // GitHub pages deployment config
  organizationName: 'Riverscapes', // GitHub org/user name
  projectName: 'RaveAddIn', // GitHub repo name

  onBrokenLinks: 'throw', // Throw an error on broken links
  onBrokenMarkdownLinks: 'warn', // Warn instead of throwing for broken markdown links

  i18n: {
    defaultLocale: 'en', // Default language
    locales: ['en'], // Supported languages
  },

  themes: ['@riverscapes/docusaurus-theme'], // Shared custom theme used across sites

  presets: [
    [
      'classic', // Docusaurus classic preset for docs/blog
      {
        docs: {
          sidebarPath: './sidebars.ts', // Path to sidebar config
          routeBasePath: '/', // Serve docs at site root
          editUrl: 'https://github.com/Riverscapes/RaveAddIn/tree/docs/', // "Edit this page" link
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'images/logo.png', // Social sharing image

    navbar: {
      title: 'Riverscapes Viewer',
      logo: {
        alt: 'Riverscapes Viewer Logo',
        src: 'images/logo.png',
      },
      items: [
        {
          to: '/about', 
          label: 'About', 
          position: 'left', 
          items: [
            { to: '/about', label: 'About Riverscapes Viewer' },
            { to: '/about/acknowledgements', label: 'Acknowledgements' },
            { to: '/about/known-bugs', label: 'Questions, Feature Requests & Bugs' },
            { to: '/about/license', label: 'License & Source Code' },
          ]
        },
        {
          to: '/deploy/install-qgis',
          label: 'Deploy',
          position: 'left',
          items: [
            { to: '/deploy/install-web', label: 'Riverscapes Viewer for the Web' },
            { to: '/deploy/install-qgis', label: 'Riverscapes Viewer for QGIS' },
            { to: '/deploy/install-arcpro', label: 'Riverscapes Viewer for ArcGIS Pro' },
          ]
        },
        {
          to: '/software-help',
          label: 'Software Help',
          position: 'left',
          items: [
            { to: '/software-help/help-web', label: 'Riverscapes Web Viewer' },
            { to: '/software-help/help-qgis', label: 'Riverscapes Viewer for QGIS' },
            { to: '/software-help/help-arcpro', label: 'Riverscapes Viewer for ArcGIS Pro' },
          ]
        },
        {
          to: '/technical-reference',
          label: 'Technical Reference',
          position: 'left',
          items: [
            { to: '/technical-reference/base-maps', label: 'Base Maps' },
            { to: '/technical-reference/business-logic', label: 'Business Logic' },
            { to: '/technical-reference/project-views', label: 'Project Views' },
            { to: '/technical-reference/symbology', label: 'Symbology' },
            { to: '/technical-reference/symbology/symbology-arc', label: 'ArcViewer Symbology' },
            { to: '/technical-reference/symbology/symbology-qgis', label: 'QViewer Symbology' },
            { to: '/technical-reference/symbology/symbology-web-vectors', label: 'Web Vector Symbology' },
            { to: '/technical-reference/symbology/symbology-web-rasters', label: 'Web Raster Symbology' },
          ]
        },
      ],
    },      

    prism: {
      theme: prismThemes.github, // Code block theme for light mode
      darkTheme: prismThemes.dracula, // Code block theme for dark mode
    },
  } satisfies Preset.ThemeConfig,
}

export default config
const menuLinks = [
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
    ]
