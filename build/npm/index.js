#!/usr/bin/env node
const { spawn } = require("child_process");
const path = require("path");

const binaryPath = path.join(__dirname, "bin", "qala");

const args = process.argv.slice(2); // Pass CLI arguments
const child = spawn(binaryPath, args, { stdio: "inherit" });

child.on("exit", process.exit);
