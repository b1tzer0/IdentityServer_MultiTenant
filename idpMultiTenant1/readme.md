Handling Migrations
Add-Migration {Name} -c ApplicationDbContext -o Data/Migrations/Application
Script-Migration {From} {To} -Context ApplicationDbContext
