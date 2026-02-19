using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Infrastructure.SkillMatching;

/// <summary>
/// In-memory skill dictionary provider.
/// Contains predefined skills with aliases for matching.
/// </summary>
public class InMemorySkillDictionary : ISkillDictionary
{
    private readonly List<SkillDefinition> _skills;

    public InMemorySkillDictionary()
    {
        _skills = InitializeSkills();
    }

    public IReadOnlyList<SkillDefinition> GetAllSkills() => _skills.AsReadOnly();

    private static List<SkillDefinition> InitializeSkills()
    {
        return new List<SkillDefinition>
        {
            // Programming Languages
            new SkillDefinition("C#", "Programming Languages", new[] { "csharp", "c-sharp", "dotnet", ".net" }),
            new SkillDefinition("Java", "Programming Languages", new[] { "java programming" }),
            new SkillDefinition("Python", "Programming Languages", new[] { "python programming" }),
            new SkillDefinition("JavaScript", "Programming Languages", new[] { "js", "javascript", "ecmascript" }),
            new SkillDefinition("TypeScript", "Programming Languages", new[] { "ts", "typescript" }),
            new SkillDefinition("C++", "Programming Languages", new[] { "cpp", "c plus plus" }),
            new SkillDefinition("Go", "Programming Languages", new[] { "golang", "go language" }),
            new SkillDefinition("Rust", "Programming Languages", new[] { "rust language" }),
            new SkillDefinition("PHP", "Programming Languages", new[] { "php programming" }),
            new SkillDefinition("Ruby", "Programming Languages", new[] { "ruby programming" }),
            new SkillDefinition("SQL", "Programming Languages", new[] { "structured query language", "t-sql", "pl/sql" }),

            // Frameworks & Libraries
            new SkillDefinition(".NET", "Frameworks", new[] { "dotnet", "asp.net", "aspnet", ".net core", ".net framework" }),
            new SkillDefinition("ASP.NET Core", "Frameworks", new[] { "asp.net core", "aspnetcore" }),
            new SkillDefinition("Entity Framework", "Frameworks", new[] { "ef", "ef core", "entity framework core" }),
            new SkillDefinition("Angular", "Frameworks", new[] { "angular.js", "angularjs" }),
            new SkillDefinition("React", "Frameworks", new[] { "react.js", "reactjs" }),
            new SkillDefinition("Vue.js", "Frameworks", new[] { "vue", "vuejs" }),
            new SkillDefinition("Node.js", "Frameworks", new[] { "nodejs", "node" }),
            new SkillDefinition("Express.js", "Frameworks", new[] { "express", "expressjs" }),
            new SkillDefinition("Spring", "Frameworks", new[] { "spring boot", "spring framework" }),
            new SkillDefinition("Django", "Frameworks", new[] { "django framework" }),
            new SkillDefinition("Flask", "Frameworks", new[] { "flask framework" }),

            // Databases
            new SkillDefinition("SQL Server", "Databases", new[] { "mssql", "microsoft sql server", "ms sql" }),
            new SkillDefinition("PostgreSQL", "Databases", new[] { "postgres", "postgresql database" }),
            new SkillDefinition("MySQL", "Databases", new[] { "mysql database" }),
            new SkillDefinition("MongoDB", "Databases", new[] { "mongo", "mongodb database" }),
            new SkillDefinition("Redis", "Databases", new[] { "redis cache" }),
            new SkillDefinition("Oracle", "Databases", new[] { "oracle database", "oracle db" }),
            new SkillDefinition("SQLite", "Databases", new[] { "sqlite database" }),

            // Cloud & DevOps
            new SkillDefinition("Azure", "Cloud & DevOps", new[] { "microsoft azure", "azure cloud" }),
            new SkillDefinition("AWS", "Cloud & DevOps", new[] { "amazon web services", "amazon aws" }),
            new SkillDefinition("Google Cloud", "Cloud & DevOps", new[] { "gcp", "google cloud platform" }),
            new SkillDefinition("Docker", "Cloud & DevOps", new[] { "docker containers", "containerization" }),
            new SkillDefinition("Kubernetes", "Cloud & DevOps", new[] { "k8s", "kubernetes orchestration" }),
            new SkillDefinition("CI/CD", "Cloud & DevOps", new[] { "continuous integration", "continuous deployment", "ci cd" }),
            new SkillDefinition("Jenkins", "Cloud & DevOps", new[] { "jenkins automation" }),
            new SkillDefinition("Git", "Cloud & DevOps", new[] { "version control", "git scm" }),
            new SkillDefinition("GitHub", "Cloud & DevOps", new[] { "github actions" }),
            new SkillDefinition("GitLab", "Cloud & DevOps", new[] { "gitlab ci" }),

            // Methodologies & Patterns
            new SkillDefinition("Agile", "Methodologies", new[] { "agile methodology", "agile development" }),
            new SkillDefinition("Scrum", "Methodologies", new[] { "scrum methodology" }),
            new SkillDefinition("Kanban", "Methodologies", new[] { "kanban methodology" }),
            new SkillDefinition("TDD", "Methodologies", new[] { "test driven development", "test-driven development" }),
            new SkillDefinition("Clean Architecture", "Methodologies", new[] { "clean code", "clean architecture pattern" }),
            new SkillDefinition("Microservices", "Methodologies", new[] { "microservice architecture", "microservices pattern" }),
            new SkillDefinition("RESTful API", "Methodologies", new[] { "rest api", "restful web services", "rest" }),
            new SkillDefinition("GraphQL", "Methodologies", new[] { "graphql api" }),

            // Testing
            new SkillDefinition("Unit Testing", "Testing", new[] { "unit tests", "xunit", "nunit", "jest" }),
            new SkillDefinition("Integration Testing", "Testing", new[] { "integration tests" }),
            new SkillDefinition("E2E Testing", "Testing", new[] { "end to end testing", "e2e tests", "selenium", "cypress" }),

            // Tools
            new SkillDefinition("Visual Studio", "Tools", new[] { "vs", "visual studio ide" }),
            new SkillDefinition("VS Code", "Tools", new[] { "visual studio code", "vscode" }),
            new SkillDefinition("IntelliJ IDEA", "Tools", new[] { "intellij" }),
            new SkillDefinition("Postman", "Tools", new[] { "postman api" }),
            new SkillDefinition("Swagger", "Tools", new[] { "swagger ui", "openapi" }),
            new SkillDefinition("JIRA", "Tools", new[] { "jira software", "atlassian jira" }),
        };
    }
}
