// Create javascript file that takes all files in dir and writes to contents to a new file like [Filename]: ```[Contents]``` .. etc
// Usage: node generate-all-ef-core-core-txt.js
// Output: all-ef-core-core.txt

const fs = require("fs");
const path = require("path");

const dir = path.join(__dirname, "EF Core");
const outputFile = path.join(__dirname, "all-ef-core-core.txt");

let output = "";

fs.readdir(dir, (err, files) => {
  if (err) {
    console.error(err);
    return;
  }

  files.forEach((file) => {
    const contents = fs.readFileSync(path.join(dir, file), "utf8");
    output += `${file}: \`\`\`${contents}\`\`\`\n\n`;
  });
});

// append IRepo.cs to the end of the file
const iRepoFile = path.join(__dirname, "IRepo.cs");
const iRepoContents = fs.readFileSync(iRepoFile, "utf8");
output += `IRepo.cs: \`\`\`${iRepoContents}\`\`\`\n\n`;

fs.writeFileSync(outputFile, output);
console.log("Done");
