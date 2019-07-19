const { task, src, context } = require("fuse-box/sparky");
const { FuseBox, QuantumPlugin, TerserPlugin } = require("fuse-box");

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
                    this.runQuantum && QuantumPlugin({
                        bakeApiIntoBundle: ["app"],
                        treeshake: true,
                    }),
                    this.runUglify && TerserPlugin()
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

    context.quiet = true;
    context.runUglify = true;

    const swFuse = context.getConfig();
    swFuse.bundle("sw")
        .cache(false)
        .instructions("> sw.ts");
    await swFuse.run();

    context.runQuantum = true;

    const appFuse = context.getConfig();
    appFuse.bundle("app")
        .cache(false)
        .instructions("> index.ts");

    await appFuse.run();
});