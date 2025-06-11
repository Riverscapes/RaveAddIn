import React from 'react'
import styles from './styles.module.css'
// import useBaseUrl from '@docusaurus/useBaseUrl'

export default function HomepageFeatures() {
  return (
    <div className={styles.container}>
      <section title="Home" className={styles.intro}>

<p>Easily explore explore <a href="https://data.riverscapes.net">riverscape projects</a> as maps, with repeatable, curated symbology and meaningful layer organization.</p>


<br/>
<h2>Consistent Cartography</h2>
<p>Stop wasting time repeatedly symbolizing layers. Riverscapes Viewers automatically applies carefully curated symbology to data, ensuring the same look and feel every time.</p>

<br/>
<h2>Logical Table of Contents</h2>
<p>Data are presented in a meaningful hierarchy that is relevant to the type data you are looking at. Layers can be added to your current map and are automatically organized into a matching table of contents.</p>

<br/>
<h2>Helpful MetaData</h2>
<p>Riverscapes Viewers make it easy to see where data originated from and navigate to the source. Metadata is available for all layers, providing context and provenance.</p>

</section>

      <Section title="Versions">
        <CardGrid>
          <ResourceCard
            title="Web Viewer"
            description="Explore riverscape projects in your web browser. No installation required."
            link="software-help/help-web"
          />
          <ResourceCard
            title="QGIS"
            description="A free plugin for QGIS."
            link="software-help/help-qgis"
          />
          <ResourceCard
            title="ArcGIS Pro"
            description="A free addin for ArcGIS Pro."
            link="software-help/help-arcpro"
          />
        </CardGrid>
      </Section>

      <Section title="Other Riverscapes Sites">
        <CardGrid>
          <ResourceCard
            title="Riverscapes Consortium"
            description="The main site for the Riverscapes Consortium."
            link="https://riverscapes.net"
          />
          <ResourceCard
            title="Riverscapes Data Exchange"
            description="A public platform for discovering, sharing, and downloading Riverscapes compliant data."
            link="https://data.riverscapes.net/"
          />
          <ResourceCard
            title="Viewer Knowledge Base"
            description="A collection of articles related to the Riverscapes Viewers."
            link="https://riverscapes.freshdesk.com/support/solutions/folders/153000068960"
          />
        </CardGrid>
      </Section>
    </div>
  )
}

function Section({ title, children }) {
  return (
    <div className={styles.section}>
      <h2>{title}</h2>
      {children}
    </div>
  )
}

function CardGrid({ children }) {
  return <div className={styles.grid}>{children}</div>
}

function ResourceCard({ title, description, link }) {
  return (
    <a href={link} className={styles.card}>
      {/* <img src={useBaseUrl('/img/card-image.jpg')} alt={title} className={styles.cardImage} /> */}
      <div className={styles.cardContent}>
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </a>
  )
}
