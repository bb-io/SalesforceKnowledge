# Blackbird.io Salesforce Knowledge

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Salesforce as a CMS or knowledge base offers robust tools for managing and disseminating content, enabling organizations to create, organize, and share information efficiently. It integrates with the CRM to provide a seamless experience for managing customer interactions and support documentation. The platform includes features like customizable templates, real-time collaboration, powerful search capabilities, and analytics to track content performance. These tools help businesses maintain a comprehensive and accessible knowledge base, improving customer service and internal knowledge management.

## Before setting up

Before you can connect you will need:

• Domain name

• Client ID

• Client secret

To get this, create a connected app on Salesforce - find instructions [here](https://help.salesforce.com/s/articleView?id=sf.connected_app_create_api_integration.htm&type=5). Walk through the steps to create a connected app, you must select the enable OAuth settings options, then select the permissions you would like to give access to to your connection. Additionally, in the redirect URI selection, fill in the following URI: `https://bridge.blackbird.io/api/AuthorizationCode`
 
Once the connected app is created, you can get the Client ID (consumer key) and Client secret (consumer secret). To access the consumer key, from the App Manager, find the connected app, and select `View` from the dropdown. Then click `Manage Consumer Details`. You’re sometimes prompted to verify your identity before you can view the consumer key.

## Connecting

1. Navigate to Apps and search for **Salesforce Knowledge**.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Salesforce'.
4. Fill in the _Domain name_, _Client ID_ and _Client secret_ obtained earlier.
5. Click Authorize connection.
6. Follow the instructions that Salesforce provides, authorizing Blackbird.io to act on your behalf.
9. When you return to Blackbird, confirm that the connection has appeared and the status is Connected.

## Actions

### Article

- **Search all master knowledge articles** Search all master knowledge articles
- **Search all published articles' translations** Search all published articles translations
- **Search all published articles** Search all published articles
- **Search knowledge article versions** Search knowledge article versions
- **Get article info** Get article info by id
- **Get articles not translated in language** Get articles not translated in specific language
- **Get all article content as object** Get all article content as object by id
- **Get article custom content as object** Get article custom content only as object by id
- **Get article content as HTML file** Get article content as HTML file by id
- **Translate knowledge article from HTML file** Translate knowledge article from HTML file
- **Submit knowledge article to translation** Submit knowledge article to translation
- **Publish knowledge article draft** Publish knowledge article draft
- **Create draft for knowledge article** Create draft for knowledge article
- **Update knowledge article field** Update knowledge article field
- **Get knowledge language settings** Get knowledge language settings

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
