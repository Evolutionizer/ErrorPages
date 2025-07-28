# Evolutionizer Error Pages

## Installation
1. Run `dotnet tool restore`
1. Run `dotnet husky install`

## Adding new error pages
Add the definition to `error-pages.json`. You can use the `{Support}` placeholder in the content to generate a mail link for the support email address.  
You can run `dotnet husky run -n compile-files` to immediately generate the new error page. It will be compiled into the `compiled` directory.

## Adjust template
Edit `template.html` according to your needs. Ensure that all referenced resources are located in `resources`. You can use the following placeholders: `{Title}`, `{Subtitle}`, `{Content}`

Do not remove the `<base>` tag as this is mandatory to ensure content is loaded from the correct directory.