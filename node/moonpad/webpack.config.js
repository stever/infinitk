const webpack = require('webpack');
const path = require('path');

module.exports = env => {
    const isProduction = env && env.production ? env.production : false;
    const srcFolder = isProduction ? 'es5' : path.join('src', 'js');
    const entryPath = path.join(__dirname, srcFolder);
    const outputFile = isProduction ? 'bundle.min.js' : 'bundle.js';
    const mainScript = 'index.js';

    const plugins = [];
    if (isProduction) {
        plugins.push(new webpack.DefinePlugin({
            'process.env.NODE_ENV': JSON.stringify('production')
        }));
    }

    plugins.push(new webpack.ProvidePlugin({
        $: 'jquery',
        jQuery: 'jquery'
    }));

    // Script loader executes JS script once in global context.
    // Useful for including jQuery plugins, for example.
    const loaders = [];

    if (!isProduction) {
        loaders.push({
            test: /\.jsx?$/,
            loader: 'babel-loader',
            exclude: /node_modules/,
            query: {
                presets: ['env']
            }
        })
    }

    return {
        mode: isProduction ? 'production' : 'development',
        devtool: isProduction ? false : 'source-map',
        output: {
            path: path.join(__dirname, 'htdocs', 'dist'),
            filename: outputFile
        },
        entry: path.join(entryPath, mainScript),
        module: {
            rules: loaders
        },
        plugins: plugins
    };
};