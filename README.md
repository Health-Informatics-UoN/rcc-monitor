# Introduction

This repository is for the NUH Collaboration projects, currently focussed on monitoring the health of instances of RedCap Cloud.

Currently this consists of an Azure Functions app that runs once a day, to retrieve the RedCap Cloud tenant level Sites from two environments, and report warnings. This is necessary, as the sites need to be identical in the two environments to enable studies to be imported between them.

## Getting Started

For now, see the `FunctionApp` directory on how to get started.
