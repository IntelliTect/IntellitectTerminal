import {
  Domain, getEnumMeta, solidify, ModelType, ObjectType,
  PrimitiveProperty, ForeignKeyProperty, PrimaryKeyProperty,
  ModelCollectionNavigationProperty, ModelReferenceNavigationProperty
} from 'coalesce-vue/lib/metadata'


const domain: Domain = { enums: {}, types: {}, services: {} }
export const CompilationLanguages = domain.enums.CompilationLanguages = {
  name: "CompilationLanguages",
  displayName: "Compilation Languages",
  type: "enum",
  ...getEnumMeta<"None"|"Python">([
  {
    value: 0,
    strValue: "None",
    displayName: "None",
  },
  {
    value: 1,
    strValue: "Python",
    displayName: "Python",
  },
  ]),
}
export const Challenge = domain.types.Challenge = {
  name: "Challenge",
  displayName: "Challenge",
  get displayProp() { return this.props.challengeId }, 
  type: "model",
  controllerRoute: "Challenge",
  get keyProp() { return this.props.challengeId }, 
  behaviorFlags: 7,
  props: {
    challengeId: {
      name: "challengeId",
      displayName: "Challenge Id",
      type: "number",
      role: "primaryKey",
      hidden: 3,
    },
    question: {
      name: "question",
      displayName: "Question",
      type: "string",
      role: "value",
    },
    level: {
      name: "level",
      displayName: "Level",
      type: "number",
      role: "value",
    },
    compilationLanguage: {
      name: "compilationLanguage",
      displayName: "Compilation Language",
      type: "enum",
      get typeDef() { return domain.enums.CompilationLanguages },
      role: "value",
    },
    answer: {
      name: "answer",
      displayName: "Answer",
      type: "string",
      role: "value",
    },
  },
  methods: {
  },
  dataSources: {
  },
}
export const Submission = domain.types.Submission = {
  name: "Submission",
  displayName: "Submission",
  get displayProp() { return this.props.submissionId }, 
  type: "model",
  controllerRoute: "Submission",
  get keyProp() { return this.props.submissionId }, 
  behaviorFlags: 7,
  props: {
    submissionId: {
      name: "submissionId",
      displayName: "Submission Id",
      type: "number",
      role: "primaryKey",
      hidden: 3,
    },
    user: {
      name: "user",
      displayName: "User",
      type: "model",
      get typeDef() { return (domain.types.User as ModelType) },
      role: "value",
      dontSerialize: true,
    },
    challenge: {
      name: "challenge",
      displayName: "Challenge",
      type: "model",
      get typeDef() { return (domain.types.Challenge as ModelType) },
      role: "value",
      dontSerialize: true,
    },
    isCorrect: {
      name: "isCorrect",
      displayName: "Is Correct",
      type: "boolean",
      role: "value",
    },
    content: {
      name: "content",
      displayName: "Content",
      type: "string",
      role: "value",
    },
  },
  methods: {
  },
  dataSources: {
  },
}
export const User = domain.types.User = {
  name: "User",
  displayName: "User",
  get displayProp() { return this.props.userId }, 
  type: "model",
  controllerRoute: "User",
  get keyProp() { return this.props.userId }, 
  behaviorFlags: 7,
  props: {
    userId: {
      name: "userId",
      displayName: "User Id",
      type: "string",
      role: "primaryKey",
      hidden: 3,
    },
    fileSystem: {
      name: "fileSystem",
      displayName: "File System",
      type: "string",
      role: "value",
    },
    creationTime: {
      name: "creationTime",
      displayName: "Creation Time",
      type: "date",
      dateKind: "datetime",
      noOffset: true,
      role: "value",
    },
  },
  methods: {
  },
  dataSources: {
  },
}
export const CommandService = domain.services.CommandService = {
  name: "CommandService",
  displayName: "Command Service",
  type: "service",
  controllerRoute: "CommandService",
  methods: {
    request: {
      name: "request",
      displayName: "Request",
      transportType: "item",
      httpMethod: "POST",
      params: {
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "string",
        role: "value",
      },
    },
    cat: {
      name: "cat",
      displayName: "Cat",
      transportType: "item",
      httpMethod: "POST",
      params: {
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
        fileName: {
          name: "fileName",
          displayName: "File Name",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "string",
        role: "value",
      },
    },
    progress: {
      name: "progress",
      displayName: "Progress",
      transportType: "item",
      httpMethod: "POST",
      params: {
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "string",
        role: "value",
      },
    },
    verify: {
      name: "verify",
      displayName: "Verify",
      transportType: "item",
      httpMethod: "POST",
      params: {
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "boolean",
        role: "value",
      },
    },
    submitFile: {
      name: "submitFile",
      displayName: "Submit File",
      transportType: "item",
      httpMethod: "POST",
      params: {
        file: {
          name: "file",
          displayName: "File",
          type: "file",
          role: "value",
        },
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "void",
        role: "value",
      },
    },
    submitUserInput: {
      name: "submitUserInput",
      displayName: "Submit User Input",
      transportType: "item",
      httpMethod: "POST",
      params: {
        input: {
          name: "input",
          displayName: "Input",
          type: "string",
          role: "value",
        },
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "void",
        role: "value",
      },
    },
  },
}
export const UserService = domain.services.UserService = {
  name: "UserService",
  displayName: "User Service",
  type: "service",
  controllerRoute: "UserService",
  methods: {
    initializeFileSystem: {
      name: "initializeFileSystem",
      displayName: "Initialize File System",
      transportType: "item",
      httpMethod: "POST",
      params: {
        userId: {
          name: "userId",
          displayName: "User Id",
          type: "string",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "model",
        get typeDef() { return (domain.types.User as ModelType) },
        role: "value",
      },
    },
  },
}

interface AppDomain extends Domain {
  enums: {
    CompilationLanguages: typeof CompilationLanguages
  }
  types: {
    Challenge: typeof Challenge
    Submission: typeof Submission
    User: typeof User
  }
  services: {
    CommandService: typeof CommandService
    UserService: typeof UserService
  }
}

solidify(domain)

export default domain as unknown as AppDomain
