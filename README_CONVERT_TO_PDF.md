# How to Convert This Guide to PDF

## Method 1: Using Visual Studio Code (Recommended)

1. Install **Visual Studio Code**: https://code.visualstudio.com/
2. Install extension: **"Markdown PDF"** by yzane
3. Open `MICROSOFT_STORE_PUBLISHING_GUIDE.md` in VS Code
4. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
5. Type "Markdown PDF: Export (pdf)"
6. Press Enter
7. PDF will be created in the same folder

## Method 2: Using Online Converter

1. Go to: https://www.markdowntopdf.com/
2. Upload `MICROSOFT_STORE_PUBLISHING_GUIDE.md`
3. Click "Convert"
4. Download the PDF

## Method 3: Using Pandoc (Command Line)

1. Install Pandoc: https://pandoc.org/installing.html
2. Open terminal in this folder
3. Run:
   ```bash
   pandoc MICROSOFT_STORE_PUBLISHING_GUIDE.md -o MICROSOFT_STORE_PUBLISHING_GUIDE.pdf
   ```

## Method 4: Using Microsoft Word

1. Open Microsoft Word
2. File → Open → Select `MICROSOFT_STORE_PUBLISHING_GUIDE.md`
3. Word will convert it automatically
4. File → Save As → PDF

---

**Recommended:** Method 1 (VS Code + Markdown PDF extension) gives the best formatting.
