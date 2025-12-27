/**
 * Pre-rendering script for Wal-O-Mat
 * 
 * Generates static HTML files for all routes to improve SEO.
 * Runs after the Blazor WASM build, before deployment.
 * 
 * Usage: node prerender.mjs [--output-dir <path>] [--base-url <url>]
 */

import { chromium } from 'playwright';
import { writeFileSync, mkdirSync, existsSync, readFileSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));

// Configuration
const config = {
  // Where the built Blazor app is located
  blazorOutputDir: process.env.BLAZOR_OUTPUT_DIR || join(__dirname, '../Client/bin/Release/net9.0/publish/wwwroot'),
  // Where to write pre-rendered files (same location to overwrite)
  outputDir: process.env.OUTPUT_DIR || join(__dirname, '../Client/bin/Release/net9.0/publish/wwwroot'),
  // Local server URL (started externally)
  baseUrl: process.env.BASE_URL || 'http://localhost:5050',
  // Timeout for page load
  timeout: 30000,
  // Wait for this selector to confirm Blazor has rendered
  waitForSelector: 'h1, .splash-container, .quiz-container, .gallery-container, .whale-detail-container, .about-container',
};

// Routes to pre-render (from sitemap.xml)
const routes = [
  { path: '/', name: 'Home' },
  { path: '/quiz', name: 'Quiz' },
  { path: '/gallery', name: 'Gallery' },
  { path: '/about-us', name: 'About' },
  // Whale detail pages
  { path: '/whale/orca', name: 'Orca' },
  { path: '/whale/blue', name: 'Blue Whale' },
  { path: '/whale/humpback', name: 'Humpback Whale' },
  { path: '/whale/beluga', name: 'Beluga Whale' },
  { path: '/whale/sperm', name: 'Sperm Whale' },
  { path: '/whale/narwhal', name: 'Narwhal' },
  { path: '/whale/gray', name: 'Gray Whale' },
  { path: '/whale/pilot', name: 'Pilot Whale' },
  { path: '/whale/minke', name: 'Minke Whale' },
  { path: '/whale/bowhead', name: 'Bowhead Whale' },
];

async function waitForBlazorReady(page) {
  // Wait for Blazor to finish loading and rendering
  await page.waitForFunction(() => {
    // Check if loading indicator is gone
    const loading = document.querySelector('.loading');
    if (loading && loading.offsetParent !== null) {
      return false;
    }
    // Check if we have actual content
    const hasContent = document.querySelector('h1, .splash-container, .gallery-container, .whale-detail-container, .about-container');
    return hasContent !== null;
  }, { timeout: config.timeout });
  
  // Extra wait for any animations/transitions
  await page.waitForTimeout(500);
}

function cleanupHtml(html) {
  // Remove Blazor reconnection UI if present
  html = html.replace(/<div id="components-reconnect-modal".*?<\/div>/gs, '');
  
  // Remove any inline Blazor error UI
  html = html.replace(/<div id="blazor-error-ui".*?<\/div>/gs, '');
  
  // Add a comment indicating this is pre-rendered
  html = html.replace('</head>', '  <!-- Pre-rendered for SEO -->\n  </head>');
  
  return html;
}

async function prerenderRoute(browser, route) {
  const page = await browser.newPage();
  
  try {
    console.log(`  Rendering ${route.name} (${route.path})...`);
    
    const url = `${config.baseUrl}${route.path}`;
    await page.goto(url, { waitUntil: 'networkidle', timeout: config.timeout });
    
    // Wait for Blazor to fully render
    await waitForBlazorReady(page);
    
    // Get the fully rendered HTML
    let html = await page.content();
    
    // Clean up the HTML
    html = cleanupHtml(html);
    
    // Determine output path
    let outputPath;
    if (route.path === '/') {
      outputPath = join(config.outputDir, 'index.html');
    } else {
      // Create directory structure: /gallery -> /gallery/index.html
      const routeDir = join(config.outputDir, route.path);
      mkdirSync(routeDir, { recursive: true });
      outputPath = join(routeDir, 'index.html');
    }
    
    // Write the pre-rendered HTML
    writeFileSync(outputPath, html, 'utf8');
    console.log(`    ✓ Saved to ${outputPath}`);
    
    return { route, success: true };
  } catch (error) {
    console.error(`    ✗ Failed: ${error.message}`);
    return { route, success: false, error: error.message };
  } finally {
    await page.close();
  }
}

async function main() {
  console.log('🐋 Wal-O-Mat Pre-rendering Script');
  console.log('==================================\n');
  
  // Parse command line arguments
  const args = process.argv.slice(2);
  for (let i = 0; i < args.length; i++) {
    if (args[i] === '--output-dir' && args[i + 1]) {
      config.outputDir = args[++i];
    } else if (args[i] === '--base-url' && args[i + 1]) {
      config.baseUrl = args[++i];
    } else if (args[i] === '--blazor-output' && args[i + 1]) {
      config.blazorOutputDir = args[++i];
    }
  }
  
  console.log(`Configuration:`);
  console.log(`  Base URL: ${config.baseUrl}`);
  console.log(`  Output Dir: ${config.outputDir}`);
  console.log(`  Routes to render: ${routes.length}\n`);
  
  // Ensure output directory exists
  if (!existsSync(config.outputDir)) {
    console.error(`Error: Output directory does not exist: ${config.outputDir}`);
    console.error('Make sure to build the Blazor app first.');
    process.exit(1);
  }
  
  console.log('Launching browser...\n');
  const browser = await chromium.launch({
    headless: true,
  });
  
  const results = [];
  
  console.log('Pre-rendering routes:');
  for (const route of routes) {
    const result = await prerenderRoute(browser, route);
    results.push(result);
  }
  
  await browser.close();
  
  // Summary
  console.log('\n==================================');
  console.log('Summary:');
  const successful = results.filter(r => r.success).length;
  const failed = results.filter(r => !r.success).length;
  console.log(`  ✓ Successful: ${successful}`);
  console.log(`  ✗ Failed: ${failed}`);
  
  if (failed > 0) {
    console.log('\nFailed routes:');
    results.filter(r => !r.success).forEach(r => {
      console.log(`  - ${r.route.path}: ${r.error}`);
    });
    process.exit(1);
  }
  
  console.log('\n🎉 Pre-rendering complete!');
}

main().catch(error => {
  console.error('Fatal error:', error);
  process.exit(1);
});

