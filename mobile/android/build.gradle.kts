allprojects {
    repositories {
        google()
        mavenCentral()
    }
}

val newBuildDir: Directory = rootProject.layout.buildDirectory.dir("../../build").get()
rootProject.layout.buildDirectory.value(newBuildDir)

subprojects {
    val newSubprojectBuildDir: Directory = newBuildDir.dir(project.name)
    project.layout.buildDirectory.value(newSubprojectBuildDir)
    
    // Fix namespace issue for older plugins like phone_state
    afterEvaluate {
        if (plugins.hasPlugin("com.android.library")) {
            extensions.getByType(com.android.build.gradle.LibraryExtension::class).apply {
                if (namespace == null) {
                    namespace = "com.badnews.${project.name}"
                }
            }
        }
    }
}
subprojects {
    project.evaluationDependsOn(":app")
}

tasks.register<Delete>("clean") {
    delete(rootProject.layout.buildDirectory)
}
