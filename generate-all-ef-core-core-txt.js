// Create javascript file that takes all files in dir and writes to contents to a new file like [Filename]: ```[Contents]``` .. etc
// Usage: node generate-all-ef-core-core-txt.js
// Output: all-ef-core-core.txt
const fs = require("fs");
const path = require("path");

const destination = "SqlKata";

const sourceDir = path.join(__dirname, "EF Core");
const destDir = path.join(__dirname, destination);
const outputFile = path.join(__dirname, "all-ef-core-code.txt");

let output = "";

try {
  fs.readdirSync(sourceDir).forEach((file) => {
    const contents = fs.readFileSync(path.join(sourceDir, file), "utf8");
    output += `${file}: \`\`\`${contents}\`\`\`\n\n`;
  });

  fs.readdirSync(destDir).forEach((file) => {
    const contents = fs.readFileSync(path.join(destDir, file), "utf8");
    output += `${file}: \`\`\`${contents}\`\`\`\n\n`;
  });

  const otherFiles = ["IRepo.cs", "schema.sql"];

  otherFiles.forEach((file) => {
    const contents = fs.readFileSync(path.join(__dirname, file), "utf8");
    output += `${file}: \`\`\`${contents}\`\`\`\n\n`;
  });

  output += `\nI'm writing a demo app to compare different data access packages. I already implemented EF Core and now I want to create similar code with ${destination}. Use LINQ for queries. Try to use one query for each method, instead of having a fetch and an update query. MS SQL Server is the database.\n`;

  fs.writeFileSync(outputFile, output);
  console.log("Done");
} catch (err) {
  console.error(err);
}
