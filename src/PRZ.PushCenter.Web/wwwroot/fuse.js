const { task, src, context } = require("fuse-box/sparky");
const { FuseBox, QuantumPlugin } = require("fuse-box");

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
                log: {
                    enabled: !this.quiet,
                },
                plugins: [
                    this.isProduction && QuantumPlugin({
                        bakeApiIntoBundle: ["app", "sw"],
                        treeshake: true,
                        uglify: true,
                    })
                ],
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

task("publish", async context => {
    await src("./dist")
        .clean("dist/")
        .exec();

    context.isProduction = true;
    context.quiet = true;

    const fuse = context.getConfig();

    fuse.bundle("app")
        .cache(false)
        .instructions("> index.ts");

    fuse.bundle("sw")
        .cache(false)
        .instructions("> sw.ts");

    await fuse.run();
});