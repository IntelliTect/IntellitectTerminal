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
  ...getEnumMeta<"None">([
  {
    value: 0,
    strValue: "None",
    displayName: "None",
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
      rules: {
        maxLength: val => !val || val.length <= 2147483647 || "Question may not be more than 2147483647 characters.",
      }
    },
    answer: {
      name: "answer",
      displayName: "Answer",
      type: "string",
      role: "value",
      rules: {
        maxLength: val => !val || val.length <= 2147483647 || "Answer may not be more than 2147483647 characters.",
      }
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
    content: {
      name: "content",
      displayName: "Content",
      type: "string",
      role: "value",
      rules: {
        maxLength: val => !val || val.length <= 2147483647 || "Content may not be more than 2147483647 characters.",
      }
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
      rules: {
        maxLength: val => !val || val.length <= 2147483647 || "File System may not be more than 2147483647 characters.",
      }
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
    requestCommand: {
      name: "requestCommand",
      displayName: "Request Command",
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
        get typeDef() { return (domain.types.Challenge as ModelType) },
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
  }
}

solidify(domain)

export default domain as unknown as AppDomain
