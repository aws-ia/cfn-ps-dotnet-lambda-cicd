#!/bin/bash
zip -r serverless-cicd.zip . -x **/.vs\* \*/bin/\* \*/obj/\* *.sh \*.ps1 .aws-sam/\*
aws --profile=dotnetquickstarts s3 cp --acl public-read serverless-cicd.zip s3://dotnet-quickstarts/source/
aws --profile=dotnetquickstarts s3 cp --acl public-read ../cfn/serverless-cicd.yml s3://dotnet-quickstarts/cloudformation/