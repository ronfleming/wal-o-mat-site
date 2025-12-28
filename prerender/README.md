# Wal-O-Mat Pre-rendering

This folder contains scripts to pre-render the Blazor WebAssembly app for SEO purposes.

## Why Pre-rendering?

Blazor WASM is a Single Page Application (SPA) - all routes return the same `index.html` file, and content is rendered by JavaScript. Search engines may not fully execute JavaScript, leading to:

- Duplicate content issues (all pages look the same to Google)
- Missing meta tags and canonical URLs
- Poor indexing

Pre-rendering generates static HTML files for each route at build time, so search engines see fully-rendered content.

## How It Works

1. **Build** - Blazor app is compiled normally with `dotnet publish`
2. **Serve** - A local HTTP server serves the built app
3. **Render** - Playwright (headless Chrome) navigates to each route and captures the rendered HTML
4. **Save** - HTML files are saved to the output directory, overwriting the original `index.html`

After pre-rendering, each route has its own complete HTML file:

```
wwwroot/
├── index.html              (pre-rendered home page)
├── quiz/
│   └── index.html          (pre-rendered quiz page)
├── gallery/
│   └── index.html          (pre-rendered gallery page)
├── whale/
│   ├── orca/
│   │   └── index.html      (pre-rendered orca page)
│   └── ...
└── _framework/             (Blazor runtime - unchanged)
```

## Local Testing

Local development is **unchanged**. Pre-rendering only runs during CI/CD builds.

To test pre-rendering locally:

```bash
# 1. Build the Blazor app
cd Client
dotnet publish -c Release

# 2. Install pre-rendering dependencies
cd ../prerender
npm install
npx playwright install chromium

# 3. Run pre-rendering
node serve-and-prerender.mjs

# 4. Check the output
ls ../Client/bin/Release/net9.0/publish/wwwroot/gallery/
# Should contain index.html
```

## Scripts

### `prerender.mjs`

The main pre-rendering script. Requires a running server.

Options:
- `--base-url <url>` - Server URL (default: `http://localhost:5050`)
- `--output-dir <path>` - Where to save HTML files

### `serve-and-prerender.mjs`

Convenience script that starts a local server, runs pre-rendering, then stops the server.

Environment variables:
- `WWWROOT` - Path to the Blazor build output

## Adding New Routes

1. Add the route to `prerender.mjs` in the `routes` array:
   ```javascript
   { path: '/new-page', name: 'New Page' },
   ```

2. Add a rewrite rule to `Client/wwwroot/staticwebapp.config.json`:
   ```json
   {
     "route": "/new-page",
     "rewrite": "/new-page/index.html"
   }
   ```

3. Add the URL to `Client/wwwroot/sitemap.xml`

## Troubleshooting

### Pre-rendering fails with timeout

The script waits for Blazor to fully render. If a page has slow loading data, increase the timeout in `prerender.mjs`:

```javascript
const config = {
  timeout: 60000, // Increase from 30000
  // ...
};
```

### Content appears different after pre-rendering

Check that the pre-rendered HTML includes Blazor's boot script. The `_framework/blazor.webassembly.js` script should hydrate the page after load.

### Missing environment variables in pre-rendered content

Pre-rendering runs against the local build, which may have different configuration than production. Ensure `wwwroot/appsettings.Production.json` is used if needed.

