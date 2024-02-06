// Create javascript file that takes all files in dir and writes to contents to a new file like [Filename]: ```[Contents]``` .. etc
// Usage: node generate-all-ef-core-core-txt.js
// Output: all-ef-core-core.txt

const fs = require("fs");
const path = require("path");

const dir = path.join(__dirname, "EF Core");
const outputFile = path.join(__dirname, "all-ef-core-code.txt");

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

  const otherFiles = [
    "IRepo.cs",
    "schema.sql",
    "Dapper/EmployeeProjectRepository.cs",
  ];

  otherFiles.forEach((file) => {
    const contents = fs.readFileSync(path.join(__dirname, file), "utf8");
    output += `${file}: \`\`\`${contents}\`\`\`\n\n`;
  });

  output += `\nI'm writing a demo app to compare different data access packages. I already implemented EF Core and now I want to create similar code with Dapper. MS SQL Server is the database.\n`;

  fs.writeFileSync(outputFile, output);
  console.log("Done");
});
