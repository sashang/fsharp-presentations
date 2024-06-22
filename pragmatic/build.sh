#!/bin/bash

if [ ! -d "node_modules" ]
then
    npm init -y
    npm i --save asciidoctor @asciidoctor/reveal.js
fi
npx asciidoctor-revealjs pragmatic.adoc