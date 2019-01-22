const { task, context } = require("fuse-box/sparky");
const { FuseBox } = require("fuse-box");

context(
    class {
        getConfig() {
            return FuseBox.init({
                homeDir: "js",
                target: "browser@es6",
                output: "dist/$name.js",
                alias: {
                    "@js": "~/",
                },
                plugins: [],
                sourceMaps: true
            });
        }
    }
);

task("default", async context => {
    const fuse = context.getConfig();

    fuse.bundle("app")
        .instructions("> index.ts")
        .watch();

    fuse.bundle("sw")
        .instructions("> sw.ts")
        .watch();

    await fuse.run();
});